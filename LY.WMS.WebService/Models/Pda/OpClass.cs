namespace LY.WMS.WebService.Models
{
    public class OpClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public OpClass()
        {
        }

        public OpClass(int paramId, string paramCode, string paramName)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
        }
    }
}