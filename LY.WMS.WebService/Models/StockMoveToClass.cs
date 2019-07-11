using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class StockMoveToClass
    {
        public List<StockMoveItemClass> ItemList;

        public string CommitKey { get; set; }

        public string WmsId { get; set; }

        public StockMoveToClass()
        {
            this.ItemList = new List<StockMoveItemClass>();
        }
    }
}