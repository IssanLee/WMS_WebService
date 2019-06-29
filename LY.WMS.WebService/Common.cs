using LY.WMS.Framework.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService
{
    public class Common
    {
        public static DBHelper OracleDB
        {
            get { return new DBHelper(DBHelper.DBType.Oracle); }
        }

        public static DBHelper MsSqlDB
        {
            get { return new DBHelper(); }
        }
    }
}