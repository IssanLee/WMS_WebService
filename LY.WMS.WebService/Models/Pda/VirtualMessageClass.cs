namespace LY.WMS.WebService.Models
{
    public abstract class VirtualMessageClass
    {
        public SessionClass Session { get; set; }

        public VirtualMessageClass()
        {
        }

        public VirtualMessageClass(SessionClass paramSession)
        {
            Session = paramSession;
        }
    }
}