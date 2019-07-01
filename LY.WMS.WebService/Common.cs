using LY.WMS.Framework.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 日期转换
        /// </summary>
        /// <param name="ParamData"></param>
        /// <returns></returns>
        public static string DateToFullStrWithFlag(DateTime ParamData)
        {
            return "'" + ParamData.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }
    }
}