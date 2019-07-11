namespace LY.WMS.WebService.Models
{
    public class UserRight
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public FuncClass Func { get; set; }

        public OpClass Op { get; set; }

        public UserRight()
        {
            Func = new FuncClass();
            Op = new OpClass();
        }

        public UserRight(int paramId, string paramCode, string paramName, FuncClass paramFunc, OpClass paramOp)
        {
            Func = new FuncClass();
            Op = new OpClass();
            Id = paramId;
            Code = paramCode;
            Name = paramName;
            Func = paramFunc;
            Op = paramOp;
        }
    }
}