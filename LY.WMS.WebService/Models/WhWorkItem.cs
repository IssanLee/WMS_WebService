using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class WhWorkItem
    {
        public int WhWorkId { get; set; }

        public EnumWorkType WhWorkType { get; set; }

        public string SourWhAreaCode { get; set; }

        public string SourLocCode { get; set; }

        public string DestWhAreaCode { get; set; }

        public string DestLocCode { get; set; }

        public string GoodsCode { get; set; }

        public string GoodsPackCode { get; set; }

        public decimal GoodsPackUnitqty { get; set; }

        public decimal GoodsPackUnitPrice { get; set; }

        public string GoodsBatchNo { get; set; }

        public string GoodsExpDate { get; set; }

        public string GoodsManuDate { get; set; }

        public decimal WorkQty { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkDate { get; set; }

        public string WorkDeviceCode { get; set; }

        public string SourTransCode { get; set; }

        public string SourTransItemId { get; set; }

        public string ShipCode { get; set; }
    }
}