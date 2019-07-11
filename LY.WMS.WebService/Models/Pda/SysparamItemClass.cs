namespace LY.WMS.WebService.Models
{
    public class SysparamItemClass
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public int GpId { get; set; }

        public string GpCode { get; set; }

        public string GpName { get; set; }

        public int ValueId { get; set; }

        public string ValueCode { get; set; }

        public string ValueName { get; set; }

        public string ValueNote { get; set; }

        public SysparamItemClass()
        {
        }

        public SysparamItemClass(int paramId, string paramCode, string paramName, string paramNote, int paramGpId, string paramGpCode, string paramGpName, int paramValueId, string paramValueCode, string paramValueName, string paramValueNote)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
            Note = paramNote;
            GpId = paramGpId;
            GpCode = paramGpCode;
            GpName = paramGpName;
            ValueId = paramValueId;
            ValueCode = paramValueCode;
            ValueName = paramValueName;
            ValueNote = paramValueNote;
        }
    }
}