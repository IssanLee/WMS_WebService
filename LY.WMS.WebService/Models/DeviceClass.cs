namespace LY.WMS.WebService.Models
{
    public class DeviceClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public DeviceClass()
        {
        }

        public DeviceClass(int paramId, string paramCode, string paramName)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
        }
    }
}