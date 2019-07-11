namespace LY.WMS.WebService.Models
{
    public class AttachItemValueClass
    {
        public int AttachItemId { get; set; }

        public int AttachItemValueId { get; set; }

        public string Value { get; set; }

        public string Note { get; set; }

        public AttachItemValueClass()
        {
        }

        public AttachItemValueClass(int paramAttachItemValueId, string paramValue, string paramNote, int paramAttachItemId)
        {
            AttachItemValueId = paramAttachItemValueId;
            Value = paramValue;
            Note = paramNote;
            AttachItemId = paramAttachItemId;
        }
    }
}