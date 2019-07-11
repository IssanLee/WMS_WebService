using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
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

    }
}