using System;

namespace LY.WMS.WebService.Models
{
    public class SessionClass
    {
        public int Id { get; set; }

        public DateTime CrDate { get; set; }

        public string CrUser { get; set; }

        public string Key { get; set; }

        public string CheckStr { get; set; }

        public string DeviceKey { get; set; }

        public SessionClass()
        {
            CrUser = "";
            Key = "";
            CheckStr = "";
            DeviceKey = "";
        }

        public SessionClass(int paramId, string paramDeviceKey)
        {
            CrUser = "";
            Key = "";
            CheckStr = "";
            DeviceKey = "";
            Id = paramId;
            DeviceKey = paramDeviceKey;
        }

        public SessionClass(int paramid, string paramDeviceKey, string paramKey, string paramCheckStr)
        {
            CrUser = "";
            Key = "";
            CheckStr = "";
            DeviceKey = "";
            Id = paramid;
            DeviceKey = paramDeviceKey;
            Key = paramKey;
            CheckStr = paramCheckStr;
        }

        public SessionClass(int paramId, string paramDeviceKey, DateTime paramCrdate, string paramCrUser, string paramKey, string paramCheckStr)
        {
            CrUser = "";
            Key = "";
            CheckStr = "";
            DeviceKey = "";
            Id = paramId;
            DeviceKey = paramDeviceKey;
            CrDate = paramCrdate;
            CrUser = paramCrUser;
            Key = paramKey;
            CheckStr = paramCheckStr;
        }
    }
}