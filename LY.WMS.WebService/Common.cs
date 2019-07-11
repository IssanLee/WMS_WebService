using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace LY.WMS.WebService
{
    public class Common
    {
        /// <summary>
        /// oracle 数据库
        /// </summary>
        public static DBHelper OracleDB
        {
            get { return new DBHelper(DBHelper.DBType.Oracle); }
        }

        /// <summary>
        /// MsSql数据库
        /// </summary>
        public static DBHelper MsSqlDB
        {
            get { return new DBHelper(); }
        }

        /// <summary>
        /// 系统参数列表
        /// </summary>
        public static SysparamListClass Sysparam = new SysparamListClass();


        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="paramOriStr"></param>
        /// <returns></returns>
        public static string PwdStrCreate(string paramOriStr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            checked
            {
                int num = paramOriStr.Length - 2;
                for (int i = 0; i <= num; i++)
                {
                    stringBuilder.Append(Math.Abs(Encoding.ASCII.GetBytes(paramOriStr.Substring(i, 1))[0] - Encoding.ASCII.GetBytes(paramOriStr.Substring(i + 1, 1))[0]).ToString());
                    stringBuilder.Append(Math.Round(unchecked((double)Encoding.ASCII.GetBytes(paramOriStr.Substring(i, 1))[0] / 2.0), 0).ToString());
                }
            }
            stringBuilder.Append(Math.Abs(Encoding.ASCII.GetBytes(paramOriStr.Substring(checked(paramOriStr.Length - 1), 1))[0] % 64).ToString());
            stringBuilder.Append(Math.Round((double)Encoding.ASCII.GetBytes(paramOriStr.Substring(checked(paramOriStr.Length - 1), 1))[0] / 2.0, 0).ToString());
            return stringBuilder.ToString();
        }


        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="ParamData"></param>
        /// <returns></returns>
        public static string DateToFullStrWithFlag(DateTime ParamData)
        {
            return "'" + ParamData.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }

        /// <summary>
        /// DbNullToString
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string IsDbNullToStr(object paramValue)
        {
            if (Convert.IsDBNull(paramValue)) return string.Empty;
            return Convert.ToString(paramValue);
        }

        /// <summary>
        /// DbNullToZero
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static int IsNullStrToZero(string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue)) return 0;
            if (Convert.IsDBNull(paramValue)) return 0;
            return Convert.ToInt32(paramValue);
        }

        /// <summary>
        /// IsNullToStr
        /// </summary>
        /// <param name="paramValue"></param>
        /// <param name="ParamToStr"></param>
        /// <returns></returns>
        public static string IsNullToStr(string paramValue, string ParamToStr = "")
        {
            if (paramValue == null)
            {
                return ParamToStr;
            }
            if (Convert.IsDBNull(paramValue))
            {
                return ParamToStr;
            }
            return paramValue;
        }

        /// <summary>
        /// IsNullNumberToZero
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static decimal IsNullNumberToZero(string paramValue)
        {
            if (paramValue == null) return 0;
            else
            {
                if (paramValue == string.Empty) return 0;
                else
                {
                    if (Convert.IsDBNull(paramValue)) return 0;
                    else return Convert.ToDecimal(paramValue);
                }
            }
        }

        #region sql参数设置共通
        /*
        /// <summary>
        /// Oracle Varchar参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="ParamDirection">输入还是输出</param>
        /// <returns></returns>
        public static OracleParameter OracleVarcharParam(string paramName, string paramValue, ParameterDirection ParamDirection = ParameterDirection.Input)
        {
            return new OracleParameter(":" + paramName, OracleDbType.NVarchar2, ParamDirection)
            {
                Value = IsNullToStr(paramValue, "")
            };
        }

        /// <summary>
        /// Oracle Int参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="ParamDirection"></param>
        /// <returns></returns>
        public static OracleParameter OracleIntParam(string paramName, int paramValue, ParameterDirection ParamDirection = ParameterDirection.Input)
        {
            return new OracleParameter(":" + paramName, OracleDbType.Int32, ParamDirection)
            {
                Value = IsNullStrToZero(Convert.ToString(paramValue))
            };
        }

        /// <summary>
        /// Oracle Number参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="ParamDirection"></param>
        /// <param name="paramScale"></param>
        /// <returns></returns>
        public static OracleParameter OracleNumberParam(string paramName, decimal paramValue, ParameterDirection ParamDirection = ParameterDirection.Input, int paramScale = 6)
        {
            return new OracleParameter(":" + paramName, OracleDbType.Decimal, ParamDirection)
            {
                Scale = checked((byte)paramScale),
                Value = IsNullNumberToZero(Convert.ToString(paramValue))
            };
        }

        /// <summary>
        /// Oracle Date参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="ParamDirection"></param>
        /// <returns></returns>
        public static OracleParameter OracleDateParam(string paramName, string paramValue, ParameterDirection ParamDirection = ParameterDirection.Input)
        {
            OracleParameter parameter = new OracleParameter(":" + paramName, OracleDbType.Date, ParamDirection);
            if (!DateTime.TryParse(paramValue, out _))
            {
                parameter.Value = DBNull.Value;
                return parameter;
            }
            parameter.Value = Convert.ToDateTime(paramValue);
            return parameter;

        }

        /// <summary>
        /// Oracle Timestamp参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="ParamDirection"></param>
        /// <returns></returns>
        public static OracleParameter OracleTimestampParam(string paramName, DateTime paramValue, ParameterDirection ParamDirection = ParameterDirection.Input)
        {
            return new OracleParameter(":" + paramName, OracleDbType.TimeStamp, ParamDirection)
            {
                Value = paramValue
            };
        }
        */
        #endregion

    }
}