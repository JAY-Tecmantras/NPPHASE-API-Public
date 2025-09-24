using NPPHASE.Data.Implementations;

namespace NPPHASE.Data.ViewModel
{
    public class SmsViewModel : AuditabeEntity
    {
        public int SMSLogId { get; set; }
        public int? DeviceUserId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string SmsType { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
    }
}
