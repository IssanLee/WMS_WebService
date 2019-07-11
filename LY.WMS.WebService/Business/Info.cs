using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Business
{
    public class Info
    {

        /// <summary>
        /// 系统信息
        /// </summary>
        /// <returns></returns>
        public static SystemInformation GetSystemInformation()
        {
            return new SystemInformation
            {
                Code = "WMS WINCE SERVICE",
                Name = "LY WMS FOR WINDOWS CE",
                Descript = "乐药WMS终端作业系统",
                Version = "1.0.0.1",
                Copyright = "广州 乐药 2018-2019 CopyRight",
                UsedBy = "医药仓储"
            };
        }

        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <returns></returns>
        public static List<SystemparamClass> GetSystemparamList()
        {
            List<SystemparamClass> list = new List<SystemparamClass>();
            string sql = string.Format(@"SELECT SYS_param_LIST_ID, CODE, NAME, DEF_VALUE_CODE FROM V_SYS_param_LIST WHERE SYS_param_GP_CODE = 'MOBILEPARAM'");
            DataTable dataTableBySql = Common.OracleDB.GetDataTableBySql(sql);
            checked
            {
                List<SystemparamClass> result;
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
                    int num = dataTableBySql.Rows.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        SystemparamClass systemparamClass = new SystemparamClass();
                        DataRow dataRow = dataTableBySql.Rows[i];
                        systemparamClass.Id = Convert.ToInt32(dataRow["SYS_PARAM_LIST_ID"]);
                        systemparamClass.Code = dataRow["CODE"].ToString();
                        systemparamClass.Name = dataRow["NAME"].ToString();
                        systemparamClass.Value = dataRow["DEF_VALUE_CODE"].ToString();
                        list.Add(systemparamClass);
                    }
                    result = list;
                }
                return result;
            }
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSystemDatetime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获取数据库时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDbDatetime()
        {
            return Convert.ToDateTime(Common.OracleDB.GetStringBySql("SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') DT FROM DUAL"));
        }

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
            catch(Exception e)
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
            catch(Exception e)
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

    }
}