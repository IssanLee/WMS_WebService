using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models.Erp
{
    public class SwapSrClass
    {
        public List<SwapSrItemClass> ItemList;

        public string SrCode { get; set; }

        public string ShipCode { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkTime { get; set; }

        public string WhId { get; set; }

        public int ItemCount { get; set; }

        public string WmsId { get; set; }

        public int StatusId { get; set; }

        public string N_1 { get; set; }

        public string N_2 { get; set; }

        public string N_3 { get; set; }

        public string N_4 { get; set; }

        public DateTime CrDate { get; set; }

        public DateTime LmDate { get; set; }

        public SwapSrClass()
        {
            this.SrCode = "";
            this.ShipCode = "";
            this.WorkByCode = "";
            this.WhId = "";
            this.WmsId = "";
            this.N_1 = "";
            this.N_2 = "";
            this.N_3 = "";
            this.N_4 = "";
            this.ItemList = new List<SwapSrItemClass>();
        }
    }
}