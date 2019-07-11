using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class StockMoveItemClass
    {
        public DateTime WorkTime { get; set; }

        public string WorkByCode { get; set; }

        public string GoodsCode { get; set; }

        public string ErpGoodsId { get; set; }

        public string BatchNo { get; set; }

        public string ExpDate { get; set; }

        public string ManuDate { get; set; }

        public string SourLocCode { get; set; }

        public string DestLocCode { get; set; }

        public decimal Qty { get; set; }

        public string UnitCode { get; set; }

        public decimal UnitQty { get; set; }

        public DateTime CrDate { get; set; }

        public DateTime LmDate { get; set; }

        public int StatusId { get; set; }

        public string WmsId { get; set; }

        public string N_1 { get; set; }

        public string N_2 { get; set; }

        public string N_3 { get; set; }

        public string N_4 { get; set; }
    }
}