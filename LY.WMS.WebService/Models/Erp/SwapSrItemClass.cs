using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models.Erp
{
    public class SwapSrItemClass
    {
        public List<EcodeItem> EcodeList;

        public string WmsId { get; set; }

        public string WhId { get; set; }

        public string SrItemId { get; set; }

        public int SrItemIndex { get; set; }

        public string GCode { get; set; }

        public string ErpGoodsId { get; set; }

        public string GpCode { get; set; }

        public decimal GpUnitQty { get; set; }

        public string BatchNo { get; set; }

        public string ExpDate { get; set; }

        public string ManuDate { get; set; }

        public decimal Qty { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkTime { get; set; }

        public string N_1 { get; set; }

        public string N_2 { get; set; }

        public string N_3 { get; set; }

        public string N_4 { get; set; }

        public string FromLocCode { get; set; }

        public string ToLocCode { get; set; }

        public DateTime CrDate { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal CheckQty { get; set; }

        public decimal RejectQty { get; set; }

        public string RejectReason { get; set; }

        public SwapSrItemClass()
        {
            this.WmsId = "";
            this.WhId = "";
            this.SrItemId = "";
            this.GCode = "";
            this.ErpGoodsId = "";
            this.GpCode = "";
            this.BatchNo = "";
            this.ExpDate = "";
            this.ManuDate = "";
            this.WorkByCode = "";
            this.N_1 = "";
            this.N_2 = "";
            this.N_3 = "";
            this.N_4 = "";
            this.FromLocCode = "";
            this.ToLocCode = "";
            this.UnitPrice = decimal.Zero;
            this.CheckQty = decimal.Zero;
            this.RejectQty = decimal.Zero;
            this.RejectReason = "";
            this.EcodeList = new List<EcodeItem>();
        }
    }
}