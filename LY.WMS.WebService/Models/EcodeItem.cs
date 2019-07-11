using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class EcodeItem
    {
        public string Ecode { get; set; }

        public string UnitCode { get; set; }

        public int UnitQty { get; set; }

        public int WmsId { get; set; }

        public EcodeItem()
        {
            this.Ecode = "";
            this.UnitCode = "";
            this.UnitQty = 1;
            this.WmsId = 0;
        }
    }
}