using System;

namespace LY.WMS.WebService.Models
{
    public class SocShipOrderClass
    {
        public int SocShipOrderId { get; set; }

        public string SoCode { get; set; }

        public string ShipOrderCode { get; set; }

        public int WholeCount { get; set; }

        public int BulkCount { get; set; }

        public int BagCount { get; set; }

        public int SpecCount { get; set; }

        public int CoolCount { get; set; }

        public int TcmCount { get; set; }

        public int TotaclCount { get; set; }

        public DateTime CrDate { get; set; }

        public string PackByName { get; set; }
    }
}