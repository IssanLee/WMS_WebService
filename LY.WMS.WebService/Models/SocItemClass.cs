using System;
using System.Collections.Generic;

namespace LY.WMS.WebService.Models
{
    public class SocItemClass
    {
        public List<EcodeItem> EcodeList;

        public string WmsId { get; set; }

        public string WhId { get; set; }

        public string SoItemId { get; set; }

        public int SoItemIndex { get; set; }

        public string GCode { get; set; }

        public string ErpGoodsId { get; set; }

        public string GpCode { get; set; }

        public decimal GpUnitQty { get; set; }

        public string BatchNo { get; set; }

        public string ExpDate { get; set; }

        public string ManuDate { get; set; }

        public decimal OrderQty { get; set; }

        public decimal Qty { get; set; }

        public decimal OffsetQty { get; set; }

        public string OffsetReason { get; set; }

        public decimal DiffQty { get; set; }

        public string FromLocCode { get; set; }

        public string ToLocCode { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkTime { get; set; }

        public string N_1 { get; set; }

        public string N_2 { get; set; }

        public string N_3 { get; set; }

        public string N_4 { get; set; }

        public DateTime CrDate { get; set; }

        public SocItemClass()
        {
            this.OffsetReason = "";
            this.EcodeList = new List<EcodeItem>();
        }
    }
}