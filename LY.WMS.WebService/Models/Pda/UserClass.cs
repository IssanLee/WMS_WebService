namespace LY.WMS.WebService.Models
{
    public class UserClass : VirtualMessageClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string LastDev { get; set; }

        public string CurrSession { get; set; }

        public UserClass()
        {
            Code = "";
            Name = "";
            LastDev = "";
        }

        public UserClass(int paramId, string paramCode, string paramName)
        {
            Code = "";
            Name = "";
            LastDev = "";
            Id = paramId;
            Code = paramCode;
            Name = paramName;
        }

        public static UserClass SessionError()
        {
            return new UserClass
            {
                Session = new SessionClass(-1, "")
            };
        }
    }
}