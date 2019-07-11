using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Business.Erp;
using LY.WMS.WebService.Models;
using LY.WMS.WebService.Models.Base;
using LY.WMS.WebService.Models.Erp;
using LY.WMS.WebService.Models.Pda;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Business.Pda
{
    /// <summary>
    /// 上架作业
    /// </summary>
    public class UpGoodsWork
    {
        private static string UpTypeStr;

        private static int ReqId;

        /// <summary>
        /// 检查目标货位
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="LocCode"></param>
        /// <returns></returns>
        public static OpResult CheckDestLoc(MobileWorkDataClass paramWorkData, string LocCode)
        {
            string sql = string.Format(@"SELECT F_CHECK_UP_LOC({0}, '{1}') FROM DUAL", paramWorkData.OrderItemId.ToString(), paramWorkData.DestLocCode);
            string checkResult = Common.OracleDB.GetStringBySql(sql);
            if (checkResult == "Success")
                return new OpResult(true, "", "", "");
            else
                return new OpResult(false, checkResult, "", "");
        }

        /// <summary>
        /// 获取订单作业明细
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        public static List<OrderWorkItemClass> GetOrderWorkList(MobileWorkDataClass paramWorkData)
        {
            string sql = string.Format(@"   SELECT T.* 
                                            FROM (
                                                SELECT ROW_NUMBER() OVER(ORDER BY W.work_time DESC) R_INDEX,W.*
                                                FROM V_WH_WORK W
                                                WHERE W.WORK_TYPE_NOTE = '{0}'
                                                AND W.WORK_TIME > SYSDATE -1
                                                AND  W.WORK_BY_ID = {1}
                                                ) T
                                            WHERE T.R_INDEX < 101",
                paramWorkData.WorkType.ToString(), paramWorkData.WorkById.ToString());
            return PdaBusiness.GetOrderWorkItemList(sql);
        }

        /// <summary>
        /// 上架数据保存
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        public static OpResult Save(MobileWorkDataClass paramWorkData)
        {
            //Log 开始保存上架数据;
            OpResult opResult = CheckWorkData(paramWorkData);
            if (!opResult.Result) return opResult;

            InitWorkData(paramWorkData);

            List<SqlItem> paramSqlItemList = new List<SqlItem>();

            // 上架作业数量模式 默认:AddQtyToAcc累加作业数量到库存数量
            if (ReqId > 0 && Common.Sysparam.GetparamValueCodeByCode("UpWorkQtyMode") != "AddQtyToAcc")
            {
                GetUpdateSourLocAccSql(paramWorkData, paramSqlItemList);
            }

            GetInsertDestLocAccSql(paramWorkData, paramSqlItemList);
            GetInsertWhWorkSql(paramWorkData, paramSqlItemList);
            GetUpdateOdItemSql(paramWorkData, paramSqlItemList);
            if (ReqId > 0)
            {
                GetUpdateGrnItemSql(paramWorkData, paramSqlItemList);
                GetUpdatePoItemSql(paramWorkData, paramSqlItemList);
                GetUpdateReqStatusSql(paramWorkData, paramSqlItemList, EnumTranStatus.UpGoods);
            }
            GetUpdateOdSql(paramWorkData, paramSqlItemList);
            SqlItemResult sqlItemResultClass = new SqlItemResult();
            try
            {
                sqlItemResultClass = Common.OracleDB.RunUpdateSqlBatchWithTran(paramSqlItemList);
            }
            catch (Exception)
            {
                return new OpResult(false, sqlItemResultClass.ErrorDescript + "\r\n" + sqlItemResultClass.ErrorMessage, "", sqlItemResultClass.ErrorSqlItem.SqlStr);
            }

            if (!sqlItemResultClass.SqlResult)
            {
                return new OpResult(false, "操作失败!\r\n" + sqlItemResultClass.ErrorDescript + "\r\n" + sqlItemResultClass.ErrorMessage, "", sqlItemResultClass.ErrorSqlItem.SqlStr.ToString());
            }
            else
            {
                if (ReqId > 0)
                {
                    if (!CheckGrnFinished(paramWorkData)) return new OpResult(true, "操作完成", "", "");

                    OpResult opResult2 = UpdatePoStatus(paramWorkData);
                    if (!opResult2.Result) return new OpResult(false, "更新采购订单状态失败", opResult2.ErrorCode, opResult2.ErrorDetail);
                }
                else if (!CheckUpFinished(paramWorkData)) return new OpResult(true, "操作完成", "", "");

                // 收货数据回传模式 默认：AfterUpFinished(整单上架完成时提交)
                // AfterReceived 收货完成时提交
                // AfterUpFinished 整单上架完成时提交
                // AfterRowUpFinished 单次上架完成时提交
                // InterFaceService 接口服务检查提交
                string receiveMode = Common.Sysparam.GetparamValueCodeByCode("ReceiveWorkUpMode");
                // ERP接口模式 默认：WebServiceComMode(WebService标准模式)
                string erpInfMode = Common.Sysparam.GetparamValueCodeByCode("ErpInterfaceMode");

                if (receiveMode != "AfterUpFinished" && erpInfMode != "WebServiceComModeWithYbx")
                {
                    return new OpResult(true, "操作完成", "", "");
                }
                else
                {
                    return StdCommSubmit(paramWorkData, UpTypeStr);
                }
            }
        }

        /// <summary>
        /// 校验作业数据
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        private static OpResult CheckWorkData(MobileWorkDataClass paramWorkData)
        {
            string resultMsg = "";

            #region 1.基础数据存在性Check
            if (paramWorkData.OrderId <= 0) resultMsg += "作业单据ID不能为空\r\n";
            if (paramWorkData.OrderItemId <= 0) resultMsg += "作业单据明细ID不能为空\r\n";
            if (string.IsNullOrEmpty(paramWorkData.DestLocCode.Trim())) resultMsg += "上货货位不能为空\r\n";
            if (paramWorkData.WorkById < 0) resultMsg += "作业人不能为空\r\n";
            #endregion

            #region 2.基础数据赋值
            if (paramWorkData.DestLocId.ToString() == "0")
            {
                paramWorkData.DestLocId = Common.OracleDB.GetIntegerBySql("SELECT L.LOC_ID FROM LOC L WHERE L.CODE = '" + paramWorkData.DestLocCode.Trim() + "'");
            }
            paramWorkData.DestGoodsId = paramWorkData.SourGoodsId;


            if (Common.Sysparam.GetparamValueCodeByCode("UpWorkAllowToAnyLoc") != "YES")
            {
                string sql = string.Format(@"SELECT V.LOC_ID FROM V_WORK_TYPE_LOC V WHERE V.WORK_TYPE_NOTE = 'UpGoodsWork' AND V.WORK_STR = 'I' AND  V.LOC_CODE = '{0}'", paramWorkData.DestLocCode.ToString());
                paramWorkData.DestLocId = Common.OracleDB.GetIntegerBySql(sql);
                if (paramWorkData.DestLocId == 0)
                {
                    resultMsg += "上货货位" + paramWorkData.DestLocCode.ToString() + "无效.\r\n";
                }
            }

            string lockMode = Common.Sysparam.GetparamValueCodeByCode("LockModeWithGoodsStorageArea");
            if (lockMode != "OnlySelf")
            {
                if (lockMode == "Contain")
                {
                    string sql = string.Format(@"SELECT F_CHK_G_LOC_STORAGE({0}) FROM DUAL", paramWorkData.DestGoodsId.ToString() + "," + paramWorkData.DestLocId.ToString());
                    string lockResult = Common.OracleDB.GetStringBySql(sql);
                    if (lockResult != "1") resultMsg += lockResult + "\r\n";
                }
            }
            else
            {
                string sql = string.Format(@"SELECT COUNT(*)
                                            FROM GOODS G
                                            INNER JOIN REF_LIST R ON G.R_3 = R.REF_LIST_ID
                                            WHERE G.GOODS_ID = {0}
                                            AND R.TEXT =  ( SELECT RL.TEXT
                                                            FROM LOC L
                                                            INNER JOIN WH_AREA  WA ON L.WH_AREA_ID = WA.WH_AREA_ID
                                                            INNER JOIN REF_LIST RL ON WA.R_3       = RL.REF_LIST_ID
                                                            WHERE L.CODE = '{1}')"
                , paramWorkData.DestGoodsId.ToString(), paramWorkData.DestLocCode.ToString());
                if (Common.OracleDB.GetIntegerBySql(sql) == 0)
                {
                    resultMsg += "上货货位" + paramWorkData.DestLocCode.ToString() + "不符合货品存储条件要求.\r\n";
                }
            }

            try
            {
                paramWorkData.DestBasePackId = Common.OracleDB.GetIntegerBySql("SELECT F_GET_GOODS_PACK_ID( " + paramWorkData.DestGoodsId.ToString() + ",'BASEPACK') FROM DUAL");
            }
            catch
            {
                resultMsg += "获取货品标准包装ID出错！\r\n";
            }


            decimal num = 0;
            try
            {
                string sql = string.Format(@"SELECT F_P1_TO_P2({0}, I.STD_PACK_ID, {1}, {2}) FROM OD_GOODS_ITEM I WHERE I.OD_GOODS_ITEM_ID = {3}",
                    paramWorkData.DestGoodsId.ToString(),
                    paramWorkData.DestBasePackId.ToString(),
                    paramWorkData.DestQty.ToString(),
                    paramWorkData.OrderItemId.ToString());
                num = new decimal(Common.OracleDB.GetIntegerBySql(sql));
            }
            catch
            {
                resultMsg += "获取货品标准包装规格出错！\r\n";
            }
            //if (decimal.Compare((int)(num), num) != 0)
            //{
            //    stringBuilder.Append("数据不符合目示货位要求,").Append("\r\n");
            //}
            paramWorkData.DestBaseQty = num;


            if (Common.OracleDB.GetStringBySql("SELECT V.R5_TEXT FROM V_LOC_L1 V WHERE V.LOC_ID = " + paramWorkData.DestLocId.ToString()) != "称重货品")
            {
                string sql = string.Format(@"SELECT MOD({0}, F_GET_LOC_PACK_UNIT_QTY({1}, {2}) ) FROM DUAL",
                    paramWorkData.DestBaseQty.ToString(), paramWorkData.DestGoodsId.ToString(), paramWorkData.DestLocId.ToString());
                if (Common.OracleDB.GetStringBySql(sql) != "0")
                {
                    resultMsg += "上货数量" + paramWorkData.DestBaseQty.ToString() + "不符合目标货位数量要求.\r\n";
                }
            }


            if ((double)Common.OracleDB.GetIntegerBySql(" SELECT COUNT(*) FROM OD O WHERE O.OD_ID =  " + paramWorkData.OrderId.ToString() + " AND O.CURR_STATUS_ID IN (1,12)") != Convert.ToDouble("1"))
            {
                resultMsg += "当前上架单据状态禁止用户上架!请检查货品是否同时重复上架.\r\n";
            }


            string checkGbQty = string.Format(@"SELECT F_CHECK_LOC_GB_QTY( {0}, {1}, {2} ) FROM DUAL",
                paramWorkData.DestLocId.ToString(), paramWorkData.DestGoodsBatchNoId.ToString(), paramWorkData.DestQty.ToString());
            string checkGbQtyResult = Common.OracleDB.GetStringBySql(checkGbQty);
            if (checkGbQtyResult != "Success")
            {
                resultMsg += checkGbQtyResult + "\r\n";
            }
            string checkLocUser = string.Format(@"SELECT F_CHECK_LOC_GOODS_USER( {0}, {1}, {2}, {3}, {4}) FROM DUAL",
                paramWorkData.DestGoodsBatchNoId.ToString(),
                paramWorkData.WorkById.ToString(),
                paramWorkData.SourLocId.ToString(),
                paramWorkData.DestLocId.ToString(),
                paramWorkData.DestQty.ToString());
            string checkLocUserResult = Common.OracleDB.GetStringBySql(checkLocUser);
            if (checkLocUserResult != "Success")
            {
                resultMsg += checkLocUserResult + "\r\n";
            }


            string upWorkMode = Common.Sysparam.GetparamValueCodeByCode("UpWorkUpMode");
            if (upWorkMode == null)
            {
                upWorkMode = "";
            }
            if (upWorkMode.ToString() != "ToRgl")
            {
                if (upWorkMode.ToString() == "ToSr")
                {
                    UpTypeStr = "SR";
                }
            }
            else
            {
                UpTypeStr = "UP";
            }
            string upWorkQtyMode = Common.Sysparam.GetparamValueCodeByCode("UpWorkQtyMode");
            if (string.IsNullOrEmpty(upWorkMode))
            {
                ReqId = Common.OracleDB.GetIntegerBySql("SELECT NVL(I.REQ_ID, 0) FROM OD I WHERE I.OD_ID = " + paramWorkData.OrderId.ToString());
                string tranCodeSql = string.Format(@"SELECT O.TRAN_CODE FROM OD O WHERE O.OD_ID = {0}", paramWorkData.OrderId.ToString());
                string tranCode = Common.OracleDB.GetStringBySql(tranCodeSql);
                if (ReqId == 0)
                {
                    if (Common.Sysparam.GetparamValueCodeByCode("YbxUpMode") == "OnlyMarkWithNote")
                    {
                        UpTypeStr = "IN";
                    }
                    else
                    {
                        switch (tranCode.Substring(0, 2).ToUpper())
                        {
                            case "IN":
                                UpTypeStr = "IN";
                                break;

                            case "UP":
                                UpTypeStr = "UP";
                                break;

                            case "AddQtyToAcc":
                                UpTypeStr = ".";
                                break;

                            case "PI":
                            case "SR":
                            case "XT":
                                UpTypeStr = "SR";
                                break;


                            default:
                                UpTypeStr = "UP";
                                break;
                        }
                        if (tranCode.Substring(0, 5).ToUpper() == "IN_SR")
                        {
                            UpTypeStr = "SR";
                        }
                    }
                }
                else
                {
                    int typeId = Common.OracleDB.GetIntegerBySql("SELECT R.TYPE_ID FROM REQ R WHERE  R.REQ_ID = " + ReqId.ToString());
                    switch (typeId)
                    {
                        case 1:
                            UpTypeStr = "PI";
                            break;
                        case 9:
                            UpTypeStr = "PO_SR";
                            break;
                        default:
                            UpTypeStr = "PI";
                            break;
                    }
                }
            }

            if (UpTypeStr == "UP" && upWorkQtyMode != "AddQtyToAcc")
            {
                string sourAccIdSql = string.Format(@"SELECT A.ACC_LOC_GBN_ID 
                                                    FROM ACC_LOC_GBN A
                                                    INNER JOIN OD_GOODS_ITEM I ON A.GOODS_ID = I.GOODS_ID AND A.GOODS_BATCH_NO_ID = I.GOODS_BATCH_NO_ID AND A.LOC_ID = I.LOC_ID AND A.QTY > 0
                                                    WHERE I.OD_GOODS_ITEM_ID = {0}", paramWorkData.OrderItemId.ToString());
                paramWorkData.SourAccId = Common.OracleDB.GetIntegerBySql(sourAccIdSql);
                if (paramWorkData.SourAccId == 0)
                {
                    resultMsg += "原货位中未能成功锁定当前指定货品和批号\r\n";
                }
            }
            #endregion

            if (resultMsg.Length == 0) return new OpResult(true, "", "", "");
            else return new OpResult(false, resultMsg, "", "");
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="paramWorkData"></param>
        private static void InitWorkData(MobileWorkDataClass paramWorkData)
        {
            paramWorkData.WorkId = Convert.ToInt32(Common.OracleDB.GetNextSeq("WH_WORK_SEQ"));
            string sql = string.Format(@"SELECT A.ACC_LOC_GBN_ID
                                        FROM ACC_LOC_GBN  A
                                        INNER JOIN OD_GOODS_ITEM I ON A.GOODS_ID = I.GOODS_ID AND A.GOODS_BATCH_NO_ID = I.GOODS_BATCH_NO_ID
                                        WHERE A.LOC_ID = '{0}'
                                        AND I.OD_GOODS_ITEM_ID = {1}
                                        AND A.WH_ID = {2}",
                paramWorkData.DestLocId.ToString(), paramWorkData.OrderItemId.ToString(), paramWorkData.DestWhId.ToString());
            paramWorkData.DestAccId = Common.OracleDB.GetIntegerBySql(sql);
            if (paramWorkData.DestAccId == 0)
            {
                paramWorkData.DestAccId = Convert.ToInt32(Common.OracleDB.GetNextSeq("ACC_LOC_GBN_SEQ"));
                paramWorkData.NewDestAccId = true;
            }
        }

        /// <summary>
        /// 获取更新源货位账目SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetUpdateSourLocAccSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE ACC_LOC_GBN U
                                        SET
                                            U.QTY     = U.QTY - {0}
                                            U.AVA_QTY = U.AVA_QTY - {1} 
                                        WHERE U.ACC_LOC_GBN_ID = {2}",
                paramWorkData.DestQty.ToString(), paramWorkData.DestQty.ToString(), paramWorkData.SourAccId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取插入目标货位账目SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetInsertDestLocAccSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql;
            if (paramWorkData.NewDestAccId)
            {
                sql = string.Format(@"INSERT INTO ACC_LOC_GBN
                                    (ACC_LOC_GBN_ID, WH_ID, LOC_ID, GOODS_ID, GOODS_PACK_ID, GOODS_BATCH_NO_ID,OD_ID,
                                        QTY, LOCK_QTY, AVA_QTY, IS_ENABLE, GID, CR_BY, CR_DATE, LM_BY, LM_DATE, ROW_VER)
                                    SELECT 
                                        {0} ACC_LOC_GBN_ID,
                                        {1} WH_ID,
                                        {2} LOC_ID,
                                        I.GOODS_ID,
                                        I.STD_PACK_ID  GOODS_PACK_ID,
                                        I.GOODS_BATCH_NO_ID,
                                        I.OD_ID,
                                        {3} QTY,
                                        0   LOCK_QTY,
                                        {3} AVA_QTY,
                                        1  IS_ENABLE,
                                        Newid(),
                                        '{4}',
                                        SYSDATE,
                                        '{4}',
                                        SYSDATE,
                                        1
                                    FROM OD_GOODS_ITEM I
                                    INNER JOIN OD O ON I.OD_ID = O.OD_ID
                                    WHERE I.OD_GOODS_ITEM_ID = {5}",
                    paramWorkData.DestAccId.ToString(),
                    paramWorkData.DestWhId.ToString(),
                    paramWorkData.DestLocId.ToString(),
                    paramWorkData.DestQty.ToString(),
                    paramWorkData.WorkById.ToString(),
                    paramWorkData.OrderItemId.ToString());
            }
            else
            {
                sql = string.Format(@"UPDATE ACC_LOC_GBN U
                                    SET
                                        U.QTY     = U.QTY     + {0}
                                        U.AVA_QTY = U.AVA_QTY + {1}
                                    WHERE U.ACC_LOC_GBN_ID = {2}",
                    paramWorkData.DestQty.ToString(), paramWorkData.DestQty.ToString(), paramWorkData.DestAccId.ToString());
            }
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取插入仓库作业SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetInsertWhWorkSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string upWorkQtyMode = Common.Sysparam.GetparamValueCodeByCode("UpWorkQtyMode");
            string sql = string.Format(@"INSERT INTO  WH_WORK
                                            (WH_WORK_ID, WORK_TYPE_ID, OD_ID, SOUR_LOC_ID, SOUR_GOODS_ID, SOUR_GOODS_PACK_ID, SOUR_GOODS_BATCH_NO_ID, SOUR_BATCH_NO, SOUR_EXP_DATE, SOUR_MANU_DATE, SOUR_QTY, DEST_LOC_ID, DEST_GOODS_ID, DEST_GOODS_PACK_ID, DEST_GOODS_BATCH_NO_ID, DEST_BATCH_NO, DEST_EXP_DATE, DEST_MANU_DATE, DEST_QTY, WORK_BY, WORK_TIME, WORK_DEV_ID, NOTE, ERPID, IS_ENABLE, GID, CR_BY, CR_DATE, LM_BY, LM_DATE, ROW_VER, SOUR_UPRICE, DEST_UPRICE, OD_GOODS_ITEM_ID)
                                        SELECT
                                            {0} WH_WORK_ID,
                                            4   WORK_TYPE_ID,
                                            I.OD_ID,", paramWorkData.WorkId.ToString());
            if (UpTypeStr == "UP" && upWorkQtyMode != "AddQtyToAcc")
                sql += string.Format(@"I.LOC_ID SOUR_LOC_ID,");
            else
                sql += string.Format(@"(SELECT LOC_ID FROM LOC L  WHERE L.CODE = '{0}' SOUR_LOC_ID," ,paramWorkData.DestLocCode);
            sql += string.Format(@" I.GOODS_ID             SOUR_GOODS_ID,
                                    I.ORDER_GOODS_PACK_ID  SOUR_GOODS_PACK_ID,
                                    I.GOODS_BATCH_NO_ID    SOUR_GOODS_BATCH_NO_ID,
                                    GB.BATCH_NO            SOUR_BATCH_NO,
                                    GB.EXP_DATE            SOUR_EXP_DATE,
                                    GB.MANU_DATE           SOUR_MANU_DATE,
                                    0                      SOUR_QTY,
                                    (SELECT LOC_ID FROM LOC L WHERE L.CODE = '{0}' DEST_LOC_ID,
                                    I.GOODS_ID             DEST_GOODS_ID,
                                    I.ORDER_GOODS_PACK_ID  DEST_GOODS_PACK_ID,
                                    I.GOODS_BATCH_NO_ID    DEST_GOODS_BATCH_NO_ID,
                                    GB.BATCH_NO            DEST_BATCH_NO,
                                    GB.EXP_DATE            DEST_EXP_DATE,
                                    GB.MANU_DATE           DEST_MANU_DATE,
                                    {1}                    DEST_QTY,
                                    '{2}',
                                    SYSDATE                WORK_TIME,
                                    ", paramWorkData.DestLocCode, paramWorkData.DestQty.ToString(), paramWorkData.WorkById.ToString());
            if (paramWorkData.WorkDeviceId == 0)
                sql += "  NULL ,";
            else
                sql += paramWorkData.WorkDeviceId.ToString() + " ,";

            sql += string.Format(@" ''                     NOTE,
                                    NULL                   ERPID,
                                    1                      IS_ENABLE,
                                    NEWID()                GID,
                                    '{0}',
                                    SYSDATE,
                                    '{0}'
                                    SYSDATE,
                                    1                      ROW_VER,
                                    0                      SOUR_UPRICE,
                                    0                      DEST_UPRICE,
                                    FROM OD_GOODS_ITEM        I
                                    INNER JOIN GOODS_BATCH_NO GB ON I.GOODS_BATCH_NO_ID = GB.GOODS_BATCH_NO_ID
                                    WHERE I.OD_GOODS_ITEM_ID = {1}
                                    ", paramWorkData.WorkById.ToString(), paramWorkData.OrderItemId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取更新单据明细SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetUpdateOdItemSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE OD_GOODS_ITEM U
                                        SET
                                            U.ARRIVED_STD_QTY   = U.ARRIVED_STD_QTY + {0},
                                            U.ARRIVED_PACK_QTY  = U.ARRIVED_PACK_QTY + {0},
                                            U.FINISHED_PACK_QTY = U.FINISHED_PACK_QTY + {0},
                                            U.LM_BY             = '{1}',
                                            U.LM_DATE           = SYSDATE,
                                            U.TO_LOC_ID         =  (SELECT LOC_ID FROM LOC L WHERE L.CODE = '{2}'),
                                            U.CURR_STATUS_ID = (CASE WHEN  U.ORDER_STD_QTY  = U.ARRIVED_STD_QTY + {0} THEN 3 ELSE 12 END)
                                        WHERE U.OD_GOODS_ITEM_ID =  {3}",
                paramWorkData.DestQty.ToString(), paramWorkData.WorkById.ToString(), paramWorkData.DestLocCode, paramWorkData.OrderItemId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取更新单据明细SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetUpdateGrnItemSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE OD_GOODS_ITEM U
                                        SET
                                            U.FINISHED_PACK_QTY = (SELECT I.FINISHED_PACK_QTY 
                                                                    FROM OD_GOODS_ITEM I 
                                                                    WHERE I.OD_GOODS_ITEM_ID = {0})
                                        WHERE U.OD_GOODS_ITEM_ID = (SELECT
                                                                        I.SOUR_OD_ID
                                                                    FROM OD_GOODS_ITEM I
                                                                    WHERE I.OD_GOODS_ITEM_ID = {1})",
                paramWorkData.OrderItemId.ToString(), paramWorkData.OrderItemId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取更新销售单明细SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetUpdatePoItemSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE REQ_ITEM U
                                        SET
                                            U.F_QTY = U.F_QTY + {0}, 
                                            U.STATUS_ID = (CASE WHEN U.F_QTY + {0} = U.W1_QTY THEN 13 ELSE 12 END)
                                        WHERE U.REQ_ITEM_ID = (
                                            SELECT 
                                                G.REQ_ITEM_ID
                                            FROM OD_GOODS_ITEM G
                                            WHERE G.OD_GOODS_ITEM_ID = {1}
                                        )", paramWorkData.DestQty.ToString(), paramWorkData.OrderItemId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 获取更新单据状态SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        /// <returns></returns>
        private static void GetUpdateReqStatusSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList, EnumTranStatus paramToStaus)
        {
            string sql = string.Format(@"UPDATE REQ U
                                        SET
                                            U.STATUS_ID = (SELECT T.STATUS_ID FROM STATUS T WHERE T.CODE = '{0}' )
                                        WHERE U.REQ_ID = (
                                            SELECT
                                                DISTINCT O.REQ_ID
                                            FROM OD_GOODS_ITEM I
                                            INNER JOIN OD O ON I.OD_ID = O.OD_ID AND O.TRAN_TYPE_ID = 11
                                            WHERE I.OD_GOODS_ITEM_ID = {1}
                                        )", paramToStaus.ToString(), paramWorkData.OrderItemId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, -1));
        }

        /// <summary>
        /// 获取更新订单SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetUpdateOdSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE OD U
                                        SET
                                            U.CURR_STATUS_ID = ( CASE WHEN (SELECT COUNT(*) FROM OD_GOODS_ITEM I WHERE I.OD_ID = U.OD_ID AND I.CURR_STATUS_ID <> 3) = 0 THEN 3 ELSE 12 END )
                                        WHERE U.OD_ID = {0}
                                        AND U.CURR_STATUS_ID IN (1, 12)", paramWorkData.OrderId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, 1));
        }

        /// <summary>
        /// 检查订单是否已完成
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        private static bool CheckGrnFinished(MobileWorkDataClass paramWorkData)
        {
            string sql = string.Format(@"SELECT
                                            COUNT(*) ITEM_COUNT
                                        FROM OD O
                                        INNER JOIN OD_GOODS_ITEM I ON O.OD_ID = I.OD_ID
                                        INNER JOIN OD            U ON O.OD_ID = U.SOUR_OD_ID
                                        WHERE U.OD_ID = {0}
                                        AND O.TRAN_TYPE_ID = 15
                                        AND (I.FINISHED_PACK_QTY <> I.ARRIVED_PACK_QTY)",
                paramWorkData.OrderId);
            try
            {
                return Common.OracleDB.GetIntegerBySql(sql) == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新采购订单状态
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramStatus"></param>
        /// <returns></returns>
        private static OpResult UpdatePoStatus(MobileWorkDataClass paramWorkData)
        {
            List<SqlItem> paramSqlItemList = new List<SqlItem>();

            GetUpdateReqStatusSql(paramWorkData, paramSqlItemList, EnumTranStatus.UpGoodsFinished);

            GetReqWorkFinishedSql(paramWorkData, paramSqlItemList);

            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.OracleDB.RunUpdateSqlBatchWithTran(paramSqlItemList);
            }
            catch (Exception ex)
            {
                return new OpResult(false, "更新订单状态出错", "", ex.Message.ToString());
            }
            if (!sqlItemResultClass.SqlResult)
            {
                return new OpResult(false, "更新订单状态失败", "", sqlItemResultClass.ErrorMessage.ToString());
            }
            else
            {
                return new OpResult(true, "操作成功", "", "");
            }
        }

        /// <summary>
        /// 获取订单完成SQL
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramSqlItemList"></param>
        private static void GetReqWorkFinishedSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"INSERT INTO REQ_WORK (REQ_ID,PARTNER_ID,WORK_TYPE_ID,FROM_STATUS_ID,TO_STATUS_ID,STATUS_DESCRIPT,NOTE,CR_BY,CR_DATE,ROW_VER)
                                        SELECT
                                            R.REQ_ID,
                                            R.PARTNER_ID,
                                            4 WORK_TYPE_ID,
                                            R.STATUS_ID FROM_STATUS_ID,
                                            13 TO_STATUS_ID,
                                            '上架完成' STATUS_DESCRIPT,
                                            '' NOTE,
                                            (SELECT S.NAME FROM STAFF S WHERE S.STAFF_ID = {0}),
                                            SYSDATE CR_DATE,
                                            0 ROW_VER
                                        FROM REQ R
                                        WHERE R.REQ_ID = 
                                            (SELECT O.REQ_ID FROM OD O WHERE O.OD_ID = {1}",
                paramWorkData.WorkById.ToString(), paramWorkData.OrderId.ToString());
            paramSqlItemList.Add(new SqlItem(sql, -1));
        }

        /// <summary>
        /// 检查上架是否完成
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        private static bool CheckUpFinished(MobileWorkDataClass paramWorkData)
        {
            return Common.OracleDB.GetIntegerBySql("SELECT CURR_STATUS_ID FROM OD O WHERE O.OD_ID =  " + paramWorkData.OrderId.ToString()) == 3;
        }

        /// <summary>
        /// 更新采购单
        /// </summary>
        /// <param name="ParamPoCode"></param>
        /// <param name="ParamResult"></param>
        /// <returns></returns>
        public static OpResult UpdatePo(string ParamPoCode, bool ParamResult)
        {
            string sql = string.Format(@"UPDATE REQ U
                                        SET
                                            U.EXP_STATUS_ID = (SELECT ES.EXP_STATUS_ID FROM EXP_STATUS ES WHERE ES.CODE = '{0}'
                                        WHERE U.CODE = '{1}'", ParamResult ? "F": "E", ParamPoCode);
            SqlItemResult sqlItemResult = Common.OracleDB.RunUpdateSqlWithTran(new SqlItem(sql, 1));
            OpResult result;
            if (!sqlItemResult.SqlResult)
            {
                result = new OpResult(false, sqlItemResult.ErrorMessage, sqlItemResult.ErrorDescript, "");
            }
            else
            {
                result = new OpResult(true, "操作成功", "", "");
            }
            return result;
        }

        /// <summary>
        /// 标准ERP接口回传
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <param name="paramUpStr"></param>
        /// <returns></returns>
        private static OpResult StdCommSubmit(MobileWorkDataClass paramWorkData, string paramUpStr)
        {
            switch (paramUpStr)
            {
                case "SR":
                case "PO_SR":
                    return SubmitSrToErp(paramWorkData);
                case "PI":
                    return SubmitRglToErp(paramWorkData);
                default:
                    return SubmitRglToErp(paramWorkData);
            }
        }

        /// <summary>
        /// 销退ERP回传
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        private static OpResult SubmitSrToErp(MobileWorkDataClass paramWorkData)
        {
            string submitSrSql = string.Format(@"SELECT V.* FROM V_SUBMIT_SR_TO_ERP V WHERE V.WMS_ID = {0}", paramWorkData.OrderId.ToString());
            DataTable subDataTable = Common.OracleDB.GetDataTableBySql(submitSrSql);
            checked
            {
                if (subDataTable == null || subDataTable.Rows.Count != 1)
                {
                    return new OpResult(false, "获取收货数据失败!", "", "");
                }
                else
                {
                    SwapSrClass swapSrClass = new SwapSrClass
                    {
                        ItemList = new List<SwapSrItemClass>()
                    };
                    DataRow dataRow = subDataTable.Rows[0];
                    swapSrClass.SrCode = dataRow["SR_CODE"].ToString();
                    swapSrClass.ShipCode = dataRow["SHIP_CODE"].ToString();
                    swapSrClass.WorkByCode = dataRow["WORK_BY_CODE"].ToString();
                    swapSrClass.WorkTime = Convert.ToDateTime(dataRow["WORK_TIME"]);
                    swapSrClass.WhId = dataRow["WH_ID"].ToString();
                    swapSrClass.ItemCount = Convert.ToInt32(dataRow["ITEM_COUNT"].ToString());
                    swapSrClass.WmsId = dataRow["WMS_ID"].ToString();
                    swapSrClass.StatusId = Convert.ToInt32(dataRow["STATUS_ID"].ToString());
                    swapSrClass.CrDate = Convert.ToDateTime(dataRow["CR_DATE"].ToString());
                    swapSrClass.LmDate = Convert.ToDateTime(dataRow["LM_DATE"].ToString());
                    swapSrClass.N_1 = dataRow["N_1"].ToString();
                    swapSrClass.N_2 = dataRow["N_2"].ToString();
                    swapSrClass.N_3 = dataRow["N_3"].ToString();
                    swapSrClass.N_4 = dataRow["N_4"].ToString();

                    string submitSrItemSql = string.Format(@"SELECT V.* FROM V_SUBMIT_SR_ITEM_TO_ERP V WHERE V.OD_ID = {0}", paramWorkData.OrderId.ToString());
                    DataTable subItemDataTable = Common.OracleDB.GetDataTableBySql(submitSrItemSql);
                    if (subItemDataTable == null || subItemDataTable.Rows.Count == 0)
                    {
                        return new OpResult(false, "获取收货明细数据失败!", "", "");
                    }
                    else
                    {
                        for (int i = 0; i < subItemDataTable.Rows.Count; i++)
                        {
                            SwapSrItemClass swapSrItemClass = new SwapSrItemClass();
                            DataRow dataRow2 = subItemDataTable.Rows[i];
                            swapSrItemClass.SrItemId = Convert.ToString(dataRow2["SR_ITEM_ID"]);
                            swapSrItemClass.SrItemIndex = Convert.ToInt32(dataRow2["SR_ITEM_INDEX"]);
                            swapSrItemClass.GCode = Convert.ToString(dataRow2["GOODS_CODE"]);
                            swapSrItemClass.ErpGoodsId = dataRow2["ERP_G_ID"].ToString();
                            swapSrItemClass.BatchNo = dataRow2["BATCH_NO"].ToString();
                            swapSrItemClass.ExpDate = dataRow2["EXP_DATE"].ToString();
                            swapSrItemClass.ManuDate = dataRow2["MANU_DATE"].ToString();
                            swapSrItemClass.GpCode = dataRow2["UNIT_CODE"].ToString();
                            swapSrItemClass.Qty = Convert.ToDecimal(dataRow2["QTY"]);
                            swapSrItemClass.GpUnitQty = Convert.ToDecimal(dataRow2["UNIT_QTY"]);
                            swapSrItemClass.WorkByCode = dataRow2["WORK_BY_CODE"].ToString();
                            swapSrItemClass.WorkTime = Convert.ToDateTime(dataRow2["WORK_TIME"]);
                            swapSrItemClass.WhId = Convert.ToString(dataRow2["WH_ID"]);
                            swapSrItemClass.WmsId = Convert.ToString(dataRow2["WMSID"]);
                            swapSrItemClass.CrDate = Convert.ToDateTime(dataRow2["CR_DATE"]);
                            swapSrItemClass.N_1 = dataRow2["N_1"].ToString();
                            swapSrItemClass.N_2 = dataRow2["N_2"].ToString();
                            swapSrItemClass.N_3 = dataRow2["N_3"].ToString();
                            swapSrItemClass.N_4 = dataRow2["N_4"].ToString();
                            swapSrItemClass.UnitPrice = Convert.ToDecimal(dataRow2["UNIT_PRICE"]);
                            swapSrItemClass.CheckQty = Convert.ToDecimal(dataRow2["CHECK_QTY"]);
                            swapSrItemClass.RejectQty = Convert.ToDecimal(dataRow2["REJECT_QTY"]);
                            swapSrItemClass.RejectReason = dataRow2["REJECT_REASON"].ToString();
                            swapSrItemClass.FromLocCode = dataRow2["FROM_LOC_CODE"].ToString();
                            swapSrItemClass.ToLocCode = dataRow2["TO_LOC_CODE"].ToString();
                            try
                            {
                                swapSrClass.ItemList.Add(swapSrItemClass);
                            }
                            catch(Exception e)
                            {
                                return new OpResult(false, "获取收货明细数据失败", "", e.Message.ToString());
                            }
                        }
                        ResultMessage resultMessage;
                        try
                        {
                            resultMessage = ErpBusiness.SubmitSrToErp(swapSrClass);
                        }
                        catch (Exception ex)
                        {
                            return new OpResult(false, "数据提交到ERP时出错", "", ex.Message.ToString());
                        }
                        if (!resultMessage.Result)
                        {
                            return new OpResult(false, "数据提交到ERP失败", "", resultMessage.ErrorStr.ToString());
                        }
                        else
                        {
                            return new OpResult(true, "操作成功", "", "");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 采购ERP回传
        /// </summary>
        /// <param name="paramWorkData"></param>
        /// <returns></returns>
        private static OpResult SubmitRglToErp(MobileWorkDataClass paramWorkData)
        {
            string submitSrSql = string.Format(@"SELECT V.* FROM V_SUBMIT_RGL_TO_ERP V WHERE V.WMS_ID = (SELECT I.SOUR_OD_ID FROM OD I WHERE I.OD_ID = {0})", paramWorkData.OrderId.ToString());
            DataTable subDataTable = Common.OracleDB.GetDataTableBySql(submitSrSql);
            checked
            {
                if (subDataTable == null || subDataTable.Rows.Count != 1)
                {
                    return new OpResult(false, "获取收货数据失败!", "回传收货入库数据失败", "");
                }
                else
                {
                    RglClass rglClass = new RglClass
                    {
                        ItemList = new List<RglItemClass>()
                    };
                    DataRow dataRow = subDataTable.Rows[0];
                    try
                    {
                        rglClass.PoCode = dataRow["PO_CODE"].ToString();
                        rglClass.ShipCode = dataRow["SHIP_CODE"].ToString();
                        rglClass.WorkByCode = dataRow["WORK_BY_CODE"].ToString();
                        rglClass.WorkTime = Convert.ToDateTime(dataRow["WORK_TIME"]);
                        rglClass.WhId = dataRow["WH_ID"].ToString();
                        rglClass.ItemCount = Convert.ToInt32(dataRow["ITEM_COUNT"].ToString());
                        rglClass.WmsId = dataRow["WMS_ID"].ToString();
                        rglClass.StatusId = Convert.ToInt32(dataRow["STATUS_ID"].ToString());
                        rglClass.CrDate = Convert.ToDateTime(dataRow["CR_DATE"].ToString());
                        rglClass.LmDate = Convert.ToDateTime(dataRow["LM_DATE"].ToString());
                        rglClass.N_1 = dataRow["N_1"].ToString();
                        rglClass.N_2 = dataRow["N_2"].ToString();
                        rglClass.N_3 = dataRow["N_3"].ToString();
                        rglClass.N_4 = dataRow["N_4"].ToString();
                    }
                    catch
                    {
                        return new OpResult(false, "填充单据头数据出错!", "回传收货入库数据失败", "");
                    }
                    string submitSrItemSql = string.Format(@"SELECT V.* FROM V_SUBMIT_RGL_ITEM_TO_ERP V WHERE V.OD_ID =(SELECT I.SOUR_OD_ID FROM OD I WHERE I.OD_ID = {0})", paramWorkData.OrderId.ToString());
                    DataTable subItemDataTable = Common.OracleDB.GetDataTableBySql(submitSrItemSql);
                    if (subItemDataTable == null || subItemDataTable.Rows.Count == 0)
                    {
                        return new OpResult(false, "获取收货明细数据失败!", "回传收货入库数据失败", "");
                    }
                    else
                    {
                        for (int i = 0; i < subItemDataTable.Rows.Count; i++)
                        {
                            RglItemClass rglItemClass = new RglItemClass();
                            DataRow dataRow2 = subItemDataTable.Rows[i];
                            try
                            {
                                rglItemClass.PoItemId = Convert.ToString(dataRow2["PO_ITEM_ID"]);
                                rglItemClass.PoItemIndex = Convert.ToInt32(dataRow2["PO_ITEM_INDEX"]);
                                rglItemClass.GCode = Convert.ToString(dataRow2["GOODS_CODE"]);
                                rglItemClass.ErpGoodsId = dataRow2["ERP_G_ID"].ToString();
                                rglItemClass.BatchNo = dataRow2["BATCH_NO"].ToString();
                                rglItemClass.ExpDate = dataRow2["EXP_DATE"].ToString();
                                rglItemClass.ManuDate = dataRow2["MANU_DATE"].ToString();
                                rglItemClass.GpCode = dataRow2["UNIT_CODE"].ToString();
                                rglItemClass.Qty = Convert.ToDecimal(dataRow2["QTY"]);
                                rglItemClass.GpUnitQty = Convert.ToDecimal(dataRow2["UNIT_QTY"]);
                                rglItemClass.WorkByCode = dataRow2["WORK_BY_CODE"].ToString();
                                rglItemClass.WorkTime = Convert.ToDateTime(dataRow2["WORK_TIME"]);
                                rglItemClass.FromLocCode = dataRow2["FROM_LOC_CODE"].ToString();
                                rglItemClass.ToLocCode = dataRow2["TO_LOC_CODE"].ToString();
                                rglItemClass.WhId = Convert.ToString(dataRow2["WH_ID"]);
                                rglItemClass.WmsId = Convert.ToString(dataRow2["WMSID"]);
                                rglItemClass.CrDate = Convert.ToDateTime(dataRow2["CR_DATE"]);
                                rglItemClass.N_1 = dataRow2["N_1"].ToString();
                                rglItemClass.N_2 = dataRow2["N_2"].ToString();
                                rglItemClass.N_3 = dataRow2["N_3"].ToString();
                                rglItemClass.N_4 = dataRow2["N_4"].ToString();
                                rglItemClass.UnitPrice = Convert.ToDecimal(dataRow2["UNIT_PRICE"]);
                                rglItemClass.CheckQty = Convert.ToDecimal(dataRow2["CHECK_QTY"].ToString());
                                rglItemClass.RejectQty = Convert.ToDecimal(dataRow2["REJECT_QTY"].ToString());
                                rglItemClass.RejectReason = dataRow2["REJECT_REASON"].ToString();
                            }
                            catch
                            {
                                return new OpResult(false, "填充收货明细数据失败!", "回传收货入库数据失败", "");
                            }
                            try
                            {
                                rglClass.ItemList.Add(rglItemClass);
                            }
                            catch
                            {
                                return new OpResult(false, "获取收货明细数据失败!", "回传收货入库数据失败", "");
                            }
                        }
                        ResultMessage resultMessage;
                        try
                        {
                            resultMessage = ErpBusiness.SubmitRglToErp(rglClass);
                        }
                        catch (Exception ex)
                        {
                            return new OpResult(false, "数据提交到ERP时出错", "回传收货入库数据失败", ex.Message.ToString());
                        }

                        UpdatePo(rglClass.PoCode, resultMessage.Result);

                        if (!resultMessage.Result)
                        {
                            return new OpResult(false, "数据提交到ERP失败", "回传收货入库数据失败", resultMessage.ErrorStr.ToString());
                        }
                        else
                        {
                            return new OpResult(true, "操作成功", "", "");
                        }
                    }
                }
            }
        }
    }
}