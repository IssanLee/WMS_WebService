using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class RglClass
    {
        public List<RglItemClass> ItemList;

        public string PoCode
        {
            get;
            set;
        }

        public string ShipCode
        {
            get;
            set;
        }

        public string WorkByCode
        {
            get;
            set;
        }

        public DateTime WorkTime
        {
            get;
            set;
        }

        public string WhId
        {
            get;
            set;
        }

        public int ItemCount
        {
            get;
            set;
        }

        public string WmsId
        {
            get;
            set;
        }

        public int StatusId
        {
            get;
            set;
        }

        public string N_1
        {
            get;
            set;
        }

        public string N_2
        {
            get;
            set;
        }

        public string N_3
        {
            get;
            set;
        }

        public string N_4
        {
            get;
            set;
        }

        public DateTime CrDate
        {
            get;
            set;
        }

        public DateTime LmDate
        {
            get;
            set;
        }

        public RglClass()
        {
            PoCode = String.Empty;
            ShipCode = String.Empty;
            WorkByCode = String.Empty;
            WhId = String.Empty;
            WmsId = String.Empty;
            N_1 = String.Empty;
            N_2 = String.Empty;
            N_3 = String.Empty;
            N_4 = String.Empty;
            ItemList = new List<RglItemClass>();
        }
    }
}