namespace LY.WMS.WebService.Models
{
    public class SystemparamClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public SystemparamClass()
        {
        }

        public SystemparamClass(int paramId, string paramCode, string paramName, string paramValue)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
            Value = paramValue;
        }
    }
}