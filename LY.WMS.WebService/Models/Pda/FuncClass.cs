namespace LY.WMS.WebService.Models
{
    public class FuncClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public FuncClass()
        {
        }

        public FuncClass(int paramId, string paramCode, string paramName)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
        }
    }
}