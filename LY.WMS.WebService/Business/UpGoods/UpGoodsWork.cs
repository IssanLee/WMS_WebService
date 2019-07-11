using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Business.UpGoods
{
    /// <summary>
    /// 上架作业
    /// </summary>
    public class UpGoodsWork
    {
        private static string UpTypeStr;

        private static int ReqId;

        public static OpResult Save(MobileWorkDataClass paramWorkData)
        {
            //Log 开始保存上架数据;
            OpResult opResult = CheckWorkData(paramWorkData);
            if (!opResult.Result) return opResult;

            InitWorkData(paramWorkData);
            List<SqlItem> paramSqlItemList = new List<SqlItem>();
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
                GetUpdateReqStatusSql(paramWorkData, paramSqlItemList);
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

                    OpResult opResult2 = UpdatePoStatus(paramWorkData, EnumTranStatusClass.UpGoodsFinished);
                    if (!opResult2.Result) return new OpResult(false, "更新采购订单状态失败", opResult2.ErrorCode, opResult2.ErrorDetail);
                }
                else if (!CheckUpFinished(paramWorkData)) return new OpResult(true, "操作完成", "", "");

                string receiveMode = Common.Sysparam.GetparamValueCodeByCode("ReceiveWorkUpMode");
                string erpInfMode = Common.Sysparam.GetparamValueCodeByCode("ErpInterfaceMode");
                if (receiveMode != "AfterUpFinished" && erpInfMode != "WebServiceComModeWithYbx")
                {
                    return new OpResult(true, "操作完成", "", "");
                }
                else if (erpInfMode != "WebServiceComMode")
                {
                    if (erpInfMode != "WebServiceComModeWithYbx")
                    {
                        if (erpInfMode != "RjtExchange")
                        {
                            return new OpResult(true, "", "", "");
                        }
                        else if (UpTypeStr != "SR" && Common.OracleDB.GetIntegerBySql("SELECT NVL(REQ_ID,0) FROM OD O WHERE O.OD_ID = " + paramWorkData.OrderId.ToString()) == 0)
                        {
                            return new OpResult(true, "操作完成", "", "");
                        }
                        else
                        {
                            return UpdatePoWithRjt(paramWorkData.OrderId, this.SubmitSrToErpWithRjtExchange(paramWorkData));
                        }
                    }
                    else
                    {
                        return YbxSubmit(paramWorkData, this.t_UpTypeStr);
                    }
                }
                else
                {
                    return StdCommSubmit(paramWorkData, this.t_UpTypeStr);
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
        private static void GetUpdateReqStatusSql(MobileWorkDataClass paramWorkData, List<SqlItem> paramSqlItemList)
        {
            string sql = string.Format(@"UPDATE REQ U
                                        SET
                                            U.STATUS_ID = (SELECT T.STATUS_ID FROM STATUS T WHERE T.CODE = '12' )
                                        WHERE U.REQ_ID = (
                                            SELECT
                                                DISTINCT O.REQ_ID
                                            FROM OD_GOODS_ITEM I
                                            INNER JOIN OD O ON I.OD_ID = O.OD_ID AND O.TRAN_TYPE_ID = 11
                                            WHERE I.OD_GOODS_ITEM_ID = {0}
                                        )", paramWorkData.OrderItemId.ToString());
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

    }
}