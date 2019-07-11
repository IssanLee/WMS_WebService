using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using LY.WMS.WebService.Models.Base;
using LY.WMS.WebService.Models.Pda;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Business.Pda
{
    /// <summary>
    /// PDA相关业务
    /// </summary>
    public class PdaBusiness
    {
        #region 0.登录权限验证部分

        #region 获取设备信息
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="paramMac">设备MAC地址</param>
        /// <param name="paramDeviceKeyCode">设备编号</param>
        /// <returns></returns>
        public static DeviceClass GetDeviceInfo(string paramMac, string paramDeviceKeyCode)
        {
            DeviceClass result;
            if (paramMac.Trim().Length == 0)
            {
                result = null;
            }
            else
            {
                DeviceClass deviceClass = new DeviceClass();
                string checkMode = Common.Sysparam.GetparamValueCodeByCode("MobileCodeCheckWith").ToUpper(); // 设备check模式
                string sql = string.Format(@"select * from v_dev where ");

                if (checkMode != "GID")
                {
                    if (checkMode != "MAC")
                    {
                        if (checkMode != "KEYCODE")
                        {
                            return null;
                        }
                        sql += string.Format(@" KEYCODE = '{0}'", Common.PwdStrCreate(paramDeviceKeyCode));
                    }
                    else
                    {
                        sql += string.Format(@" MAC_ADDR = '{0}'", paramMac.ToString());
                    }
                }
                else
                {
                    sql += string.Format(@" GID = '{0}'", paramDeviceKeyCode.ToString());
                }
                DataTable dataTableBySql = Common.OracleDB.GetDataTableBySql(sql);
                if (dataTableBySql == null)
                {
                    result = null;
                }
                else if (dataTableBySql.Rows.Count != 1)
                {
                    result = null;
                }
                else
                {
                    DataRow dataRow = dataTableBySql.Rows[0];
                    deviceClass.Id = Convert.ToInt32(dataRow["DEV_ID"]);
                    deviceClass.Code = dataRow["NOTE"].ToString();
                    deviceClass.Name = dataRow["NOTE"].ToString();
                    result = deviceClass;
                }
            }
            return result;
        }
        #endregion

        #region 唯一码登录
        /// <summary>
        /// 唯一码登录
        /// </summary>
        /// <param name="paramCardPwd">唯一码</param>
        /// <param name="paramDeviceMac">设备MAC</param>
        /// <returns></returns>
        public static UserClass CardPwdLogin(string paramCardPwd, string paramDeviceMac)
        {
            UserClass result;
            if (paramCardPwd == string.Empty)
            {
                result = null;
            }
            else
            {
                UserClass userClass = new UserClass();
                string sql = string.Format(@"SELECT * FROM V_USER U WHERE U.CARD_PWD  = :CARD_PWD");

                List<DbParameter> list = new List<DbParameter>
                {
                    new OracleParameter(":CARD_PWD", OracleDbType.NVarchar2)
                    {
                        Value = paramCardPwd
                    }
                };
                DataTable dataTableBySqlWithParam = Common.OracleDB.GetDataTableBySqlWithParam(sql, list);
                if (dataTableBySqlWithParam == null)
                {
                    result = null;
                }
                else if (dataTableBySqlWithParam.Rows.Count != 1)
                {
                    result = null;
                }
                else
                {
                    userClass.Id = Convert.ToInt32(dataTableBySqlWithParam.Rows[0]["STAFF_ID"]);
                    userClass.Code = Convert.ToString(dataTableBySqlWithParam.Rows[0]["CODE"]);
                    userClass.Name = Convert.ToString(dataTableBySqlWithParam.Rows[0]["NAME"]);
                    userClass.LastDev = paramDeviceMac;

                    if (CreateUserSession(userClass))
                    {
                        result = userClass;
                    }
                    else
                    {
                        result = UserClass.SessionError();
                    }
                }
            }
            return result;
        }
        #endregion

        #region 获取用户权限
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="paramUserFlag"></param>
        /// <param name="paramUserGid"></param>
        /// <returns></returns>
        public static List<UserRight> GetUserRightList(string paramUserFlag, string paramUserGid)
        {
            int userId = GetUserId(Convert.ToInt32(paramUserFlag));

            List<UserRight> result;
            if (userId == 0)
            {
                result = null;
            }
            else
            {
                List<UserRight> list = new List<UserRight>();

                string sql = string.Format(@"SELECT V.RIGHT_LIST_ID, V.RIGHT_CODE, V.RIGHT_NAME, V.FUNC_ID, V.FUNC_CODE, V.FUNC_NAME, V.OP_ID, V.OP_CODE, V.OP_NAME
                                            FROM V_USER_RIGHT_LIST V
                                            WHERE V.FUNC_TYPE_TEXT = 'MobileFunction'
                                            AND V.STAFF_ID = :STAFF_ID");
                List<DbParameter> parameterList = new List<DbParameter>
                {
                    new OracleParameter(":STAFF_ID", OracleDbType.Int32)
                    {
                        Value = userId
                    }
                };

                DataTable dataTable = new DataTable();
                try
                {
                    dataTable = Common.OracleDB.GetDataTableBySqlWithParam(sql, parameterList);
                }
                catch (Exception ex)
                {
                }
                if (dataTable == null)
                {
                    result = null;
                }
                else
                {
                    int num = dataTable.Rows.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        UserRight userRight = new UserRight();
                        DataRow dataRow = dataTable.Rows[i];
                        userRight.Id = Convert.ToInt32(dataRow["RIGHT_LIST_ID"]);
                        userRight.Code = Convert.ToString(dataRow["RIGHT_CODE"]);
                        userRight.Name = Convert.ToString(dataRow["FUNC_NAME"]);
                        userRight.Func = new FuncClass(Convert.ToInt32(dataRow["FUNC_ID"]), Convert.ToString(dataRow["FUNC_CODE"]), Convert.ToString(dataRow["FUNC_NAME"]));
                        userRight.Op = new OpClass(Convert.ToInt32(dataRow["OP_ID"]), Convert.ToString(dataRow["OP_CODE"]), Convert.ToString(dataRow["OP_NAME"]));
                        list.Add(userRight);
                    }
                    result = list;
                }
            }
            return result;
        }
        #endregion

        #region 获取用户ID
        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="paramUserFlag"></param>
        /// <returns></returns>
        public static int GetUserId(int paramUserFlag)
        {
            string sql = string.Format(@"SELECT USER_ID FROM V_USER_SESSION V WHERE V.USER_SESSION_ID = :USER_SESSION_ID");
            List<DbParameter> list = new List<DbParameter>
            {
                new OracleParameter(":USER_SESSION_ID", OracleDbType.Int32)
                {
                    Value = paramUserFlag
                }
            };
            return Common.OracleDB.GetIntegerBySqlWithParam(sql, list);
        }
        #endregion


        #region 创建用户Session
        /// <summary>
        /// 创建用户Session
        /// </summary>
        /// <param name="paramUser"></param>
        /// <returns></returns>
        private static bool CreateUserSession(UserClass paramUser)
        {
            List<SqlParamItem> list = new List<SqlParamItem>();

            string delSql = string.Format(@"DELETE FROM USER_SESSION U WHERE U.USER_ID =  :USER_ID");
            List<DbParameter> parameterList1 = new List<DbParameter>
            {
                new OracleParameter(":USER_ID", OracleDbType.NVarchar2, 100)
                {
                    Value = paramUser.Id
                }
            };
            list.Add(new SqlParamItem(delSql, parameterList1, -1, -1));

            List<DbParameter> parameterList2 = new List<DbParameter>();

            string istSql = string.Format(@"INSERT INTO  USER_SESSION (USER_ID,DEV_MAC) Values (:USER_ID, :DEV_MAC)");
            parameterList2.Add(new OracleParameter(":USER_ID", OracleDbType.NVarchar2, 100)
            {
                Value = paramUser.Id
            });
            parameterList2.Add(new OracleParameter(":DEV_MAC", OracleDbType.NVarchar2, 100)
            {
                Value = paramUser.LastDev
            });
            list.Add(new SqlParamItem(istSql, parameterList2, 1, -1));



            SqlParamItemResult sqlParamItemResultClass;
            bool result;
            try
            {
                sqlParamItemResultClass = Common.OracleDB.RunProcBatchWithParamWithTran(list);
            }
            catch (Exception e)
            {
                return false;
            }

            string querySql = string.Format(@"SELECT V.USER_SESSION_ID FROM V_USER_SESSION V WHERE V.USER_ID = :USER_ID");

            List<DbParameter> parameterList3 = new List<DbParameter>
            {
                new OracleParameter(":USER_ID", OracleDbType.Int32)
                {
                    Value = paramUser.Id
                }
            };
            int integerBySqlWithParam;
            try
            {
                integerBySqlWithParam = Common.OracleDB.GetIntegerBySqlWithParam(querySql, parameterList3);
            }
            catch (Exception e)
            {
                return false;
            }
            if (integerBySqlWithParam == 0)
            {
                result = false;
            }
            else
            {
                paramUser.CurrSession = integerBySqlWithParam.ToString();
                result = sqlParamItemResultClass.SqlResult;
            }
            return result;
        }
        #endregion

        #region Check Session
        /// <summary>
        /// Check Session
        /// </summary>
        /// <param name="paramUserFlag"></param>
        /// <returns></returns>
        public static bool CheckUserSession(int paramUserFlag)
        {
            string sql = string.Format(@"SELECT USER_ID FROM V_USER_SESSION V WHERE V.USER_SESSION_ID = :USER_SESSION_ID");
            List<DbParameter> list = new List<DbParameter>
            {
                new OracleParameter(":USER_SESSION_ID", OracleDbType.Int32)
                {
                    Value = paramUserFlag
                }
            };
            return Common.OracleDB.GetIntegerBySqlWithParam(sql, list) != 0;
        }
        #endregion

        #region 

        #endregion

        #endregion

        #region 1.其他查询
        /// <summary>
        /// 获取订单作业明细
        /// </summary>
        /// <param name="paramGetSql"></param>
        /// <returns></returns>
        public static List<OrderWorkItemClass> GetOrderWorkItemList(string paramGetSql)
        {
            List<OrderWorkItemClass> list = new List<OrderWorkItemClass>();
            DataTable dataTableBySql = Common.OracleDB.GetDataTableBySql(paramGetSql.ToString());
            checked
            {
                if (dataTableBySql == null) return null;
                else if (dataTableBySql.Rows.Count == 0) return null;
                else
                {
                    for (int i = 0; i < dataTableBySql.Rows.Count; i++)
                    {
                        GoodsClass goodsClass = new GoodsClass()
                        {
                            Id = Convert.ToInt32(dataTableBySql.Rows[i]["DEST_GOODS_ID"]),
                            Code = dataTableBySql.Rows[i]["GOODS_CODE"].ToString(),
                            Name = dataTableBySql.Rows[i]["GOODS_NAME"].ToString(),
                            Spec = dataTableBySql.Rows[i]["GOODS_SPEC"].ToString(),
                            Prod = dataTableBySql.Rows[i]["GOODS_MANU"].ToString(),
                            Manu = dataTableBySql.Rows[i]["GOODS_MANU"].ToString(),
                            ApprNo = dataTableBySql.Rows[i]["APPR_NO"].ToString()
                        };

                        GoodsPackClass goodsPackClass = new GoodsPackClass()
                        {
                            Id = Convert.ToInt32(dataTableBySql.Rows[i]["DEST_GOODS_PACK_ID"]),
                            Code = dataTableBySql.Rows[i]["GOODS_PACK_CODE"].ToString(),
                            Name = dataTableBySql.Rows[i]["GOODS_PACK_NAME"].ToString(),
                            UnitQty = Convert.ToDecimal(dataTableBySql.Rows[i]["GOODS_PACK_UNIT_QTY"]),
                            PackBcode = dataTableBySql.Rows[i]["PACK_BCODE"].ToString(),
                            Goods = goodsClass
                        };

                        OrderWorkItemClass orderWorkItemClass = new OrderWorkItemClass()
                        {
                            WorkQty = Convert.ToDecimal(dataTableBySql.Rows[i]["DEST_QTY"]),
                            WorkUnitPrice = Convert.ToDecimal(dataTableBySql.Rows[i]["DEST_UPRICE"]),
                            DestLocCode = dataTableBySql.Rows[i]["DEST_LOC_CODE"].ToString(),
                            SourLocCode = dataTableBySql.Rows[i]["SOUR_LOC_CODE"].ToString(),
                            BatchNo = dataTableBySql.Rows[i]["DEST_BATCH_NO"].ToString(),
                            ExpDate = dataTableBySql.Rows[i]["DEST_EXP_DATE"].ToString(),
                            WorkByName = dataTableBySql.Rows[i]["WORK_BY_NAME"].ToString(),
                            WorkDate = Convert.ToDateTime(dataTableBySql.Rows[i]["WORK_TIME"]),
                            WorkDeviceCode = dataTableBySql.Rows[i]["WORK_DEVICE_NOTE"].ToString(),
                            GoodsPack = goodsPackClass
                        };
                        list.Add(orderWorkItemClass);
                    }
                    return list;
                }
            }
        }
        #endregion

        #region 2.上架部分

        #region 获取待上架货品信息
        /// <summary>
        /// 获取待上架货品信息
        /// </summary>
        /// <param name="paramUserFlag">登录用户Session</param>
        /// <param name="paramGoodsStr">货品信息</param>
        /// <param name="paramWhId">仓库ID(没有作用但是必需的)</param>
        /// <returns></returns>
        public static List<UpGoodsBatchNoClass> GetUpGoodsBatchNoList(string paramUserFlag, string paramGoodsStr, int paramWhId = 0)
        {
            checked
            {
                List<UpGoodsBatchNoClass> result;
                if (!CheckUserSession(Convert.ToInt32(paramUserFlag)))
                {
                    result = null;
                }
                else
                {
                    string sql = string.Format(@"SELECT
                                                    ROW_NUMBER() OVER(ORDER BY I.OD_ID, I.GOODS_ID, I.GOODS_BATCH_NO_ID) ROW_INDEX,
                                                    O.TRAN_CODE,
                                                    F_GET_OD_ITEM_UP_TYPE(I.OD_GOODS_ITEM_ID) REQ_TYPE,
                                                    G.CODE GOODS_CODE,
                                                    G.NAME GOODS_NAME,
                                                    G.SPEC,
                                                    G.PROD,
                                                    G.MANU,
                                                    G.APPR_NO,
                                                    GP.PACK_BCODE,
                                                    GB.BATCH_NO,
                                                    GB.EXP_DATE,
                                                    GB.MANU_DATE,
                                                    GP.CODE PACK_CODE,
                                                    GP.pack_qty PACK_UNIT_QTY,
                                                    f_get_box_to_base_qty(I.GOODS_ID,1) BOX_PACK_UNIT_QTY,
                                                    (SELECT WP.CODE FROM GOODS_PACK WP WHERE WP.GOODS_ID = I.GOODS_ID AND WP.PACK_TYPE_ID = 4) BOX_PACK_CODE,
                                                    GP.NAME PACK_NAME,
                                                    (SELECT MAX(WA.NOTE) FROM WH_AREA WA INNER JOIN REQ_ITEM RI ON WA.ERPID = RI.WH_ERPID WHERE RI.REQ_ITEM_ID = I.REQ_ITEM_ID ) WH_ERP_NAME,
                                                    G.GOODS_ID,
                                                    GP.GOODS_PACK_ID,
                                                    GB.GOODS_BATCH_NO_ID,
                                                    F_GET_UP_SUGG_TO_BULK_LOC(I.OD_GOODS_ITEM_ID,I.ORDER_STD_QTY) TO_LOC_CODE,
                                                    0  TO_LOC_ID,
                                                    I.ORDER_STD_QTY - I.FINISHED_PACK_QTY ORDER_STD_QTY,
                                                    G.STORAGE_INFO,
                                                    I.OD_ID,
                                                    I.OD_GOODS_ITEM_ID
                                                FROM OD O
                                                INNER JOIN OD_GOODS_ITEM  I  ON O.OD_ID = I.OD_ID
                                                INNER JOIN V_GOODS G  ON I.GOODS_ID = G.GOODS_ID
                                                INNER JOIN GOODS_PACK GP ON I.ORDER_GOODS_PACK_ID = GP.GOODS_PACK_ID
                                                INNER JOIN GOODS_BATCH_NO GB ON I.GOODS_BATCH_NO_ID = GB.GOODS_BATCH_NO_ID

                                                LEFT JOIN V_BULK_GOODS_UP_CURR_LOC  A  ON  I.GOODS_ID = A.GOODS_ID  AND  I.GOODS_BATCH_NO_ID  =  A.GOODS_BATCH_NO_ID
                                                WHERE O.TRAN_TYPE_ID IN(9, 11)
                                                AND I.CURR_STATUS_ID IN (1,12)
                                                AND O.CR_DATE > SYSDATE - 30
                                                AND (  G.CODE LIKE '%{0}%'
                                                    OR GP.PACK_BCODE LIKE  '%{0}%'
                                                    OR G.APPR_NO     LIKE  '%{0}%'
                                                    OR G.SEAR_CODE_1 LIKE  UPPER('%{0}%')
                                                    OR G.SEAR_CODE_1 LIKE  LOWER('%{0}%')
                                                )", paramGoodsStr);
                    DataTable dataTableBySql = Common.OracleDB.GetDataTableBySql(sql);
                    if (dataTableBySql == null)
                    {
                        result = null;
                    }
                    else if (dataTableBySql.Rows.Count == 0)
                    {
                        result = null;
                    }
                    else
                    {
                        List<UpGoodsBatchNoClass> list = new List<UpGoodsBatchNoClass>();
                        for (int i = 0; i < dataTableBySql.Rows.Count; i++)
                        {
                            UpGoodsBatchNoClass upGoodsBatchNoClass = new UpGoodsBatchNoClass();
                            DataRow dataRow = dataTableBySql.Rows[i];
                            upGoodsBatchNoClass.TranCode = dataRow["TRAN_CODE"].ToString();
                            upGoodsBatchNoClass.Reqtype = dataRow["REQ_TYPE"].ToString();
                            upGoodsBatchNoClass.GoodsCode = dataRow["GOODS_CODE"].ToString();
                            upGoodsBatchNoClass.GoodsName = dataRow["GOODS_NAME"].ToString();
                            upGoodsBatchNoClass.Spec = dataRow["SPEC"].ToString();
                            upGoodsBatchNoClass.Prod = dataRow["PROD"].ToString();
                            upGoodsBatchNoClass.Manu = dataRow["MANU"].ToString();
                            upGoodsBatchNoClass.ApprNo = dataRow["APPR_NO"].ToString();
                            upGoodsBatchNoClass.PackCode = dataRow["PACK_CODE"].ToString();
                            upGoodsBatchNoClass.PackName = dataRow["PACK_NAME"].ToString();
                            upGoodsBatchNoClass.PackUnitQty = Convert.ToDecimal(dataRow["PACK_UNIT_QTY"]);
                            upGoodsBatchNoClass.BoxPackUnitQty = Convert.ToDecimal(dataRow["BOX_PACK_UNIT_QTY"]);
                            upGoodsBatchNoClass.BoxPackCode = dataRow["BOX_PACK_CODE"].ToString();
                            upGoodsBatchNoClass.Barcode = dataRow["PACK_BCODE"].ToString();
                            upGoodsBatchNoClass.BatchNo = dataRow["BATCH_NO"].ToString();
                            upGoodsBatchNoClass.ExpDate = dataRow["EXP_DATE"].ToString();
                            upGoodsBatchNoClass.ManuDate = dataRow["MANU_DATE"].ToString();
                            upGoodsBatchNoClass.Qty = Convert.ToDecimal(dataRow["ORDER_STD_QTY"].ToString());
                            upGoodsBatchNoClass.ToLocCode = dataRow["TO_LOC_CODE"].ToString();
                            upGoodsBatchNoClass.ErpWhName = dataRow["WH_ERP_NAME"].ToString();
                            upGoodsBatchNoClass.StorageCondition = dataRow["STORAGE_INFO"].ToString();
                            upGoodsBatchNoClass.OrderId = Convert.ToInt32(dataRow["OD_ID"].ToString());
                            upGoodsBatchNoClass.OrderItemId = Convert.ToInt32(dataRow["OD_GOODS_ITEM_ID"].ToString());
                            upGoodsBatchNoClass.ToLocId = Convert.ToInt32(dataRow["TO_LOC_ID"].ToString());
                            upGoodsBatchNoClass.GoodsId = Convert.ToInt32(dataRow["GOODS_ID"].ToString());
                            upGoodsBatchNoClass.GoodsPackId = Convert.ToInt32(dataRow["GOODS_PACK_ID"].ToString());
                            upGoodsBatchNoClass.GoodsBatchNoId = Convert.ToInt32(dataRow["GOODS_BATCH_NO_ID"].ToString());
                            list.Add(upGoodsBatchNoClass);
                        }
                        result = list;
                    }
                }
                return result;
            }
        }
        #endregion

        #region 

        #endregion

        #endregion

        
    }
}