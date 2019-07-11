using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class SocClass
    {
        public List<SocItemClass> ItemList;

        public List<SocShipOrderClass> ShipOrderList;

        public string SoCode { get; set; }

        public string ShipCode { get; set; }

        public string WorkByCode { get; set; }

        public DateTime WorkTime { get; set; }

        public string WhId { get; set; }

        public int ItemCount { get; set; }

        public int ShipOrderCount { get; set; }

        public string WmsId { get; set; }

        public int StatusId { get; set; }

        public string N_1 { get; set; }

        public string N_2 { get; set; }

        public string N_3 { get; set; }

        public string N_4 { get; set; }

        public DateTime CrDate { get; set; }

        public DateTime LmDate { get; set; }

        public SocClass()
        {
            this.ItemList = new List<SocItemClass>();
            this.ShipOrderList = new List<SocShipOrderClass>();
        }
    }
}