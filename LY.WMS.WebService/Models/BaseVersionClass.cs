using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class BaseVersionClass
    {
        public string CrBy
        {
            get;
            set;
        }

        public DateTime CrDate
        {
            get;
            set;
        }

        public string LmBy
        {
            get;
            set;
        }

        public DateTime LmDate
        {
            get;
            set;
        }

        public int RowVersion
        {
            get;
            set;
        }

        public string Gid
        {
            get;
            set;
        }

        public BaseVersionClass()
        {
            Gid = String.Empty;
        }

        public BaseVersionClass(string ParamCrBy, DateTime ParamCrDate, string ParamLmBy, DateTime ParamLmDate, int ParamRowVer)
        {
            Gid = String.Empty;
            CrBy = ParamCrBy;
            CrDate = ParamCrDate;
            LmBy = ParamLmBy;
            LmDate = ParamLmDate;
            RowVersion = ParamRowVer;
        }
    }
}