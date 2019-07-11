using System;

namespace LY.WMS.WebService.Models
{
    public class ErpAccLocGbnClass
    {
        public string WH_CODE { get; set; }

        public string LOC_CODE { get; set; }

        public string G_CODE { get; set; }

        public string ERP_G_ID { get; set; }

        public string BATCH_NO { get; set; }

        public string GP_CODE { get; set; }

        public decimal QTY { get; set; }

        public string CR_BY { get; set; }

        public DateTime CR_DATE { get; set; }

        public string LM_BY { get; set; }

        public DateTime LM_DATE { get; set; }

        public int WMS_ID { get; set; }

        public int ROW_VER { get; set; }
    }
}