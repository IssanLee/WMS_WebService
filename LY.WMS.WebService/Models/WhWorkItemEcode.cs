using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class WhWorkItemEcode
    {
        public EnumEcodeWorkType EnumWorkType { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkDate { get; set; }

        public int WhWorkId { get; set; }

        public string SourWhAreaCode { get; set; }

        public string SourTransCode { get; set; }

        public string SourTransItemId { get; set; }

        public string Ecode { get; set; }

        public EnumEcodePackLevel EcodePackLevel { get; set; }
    }
}