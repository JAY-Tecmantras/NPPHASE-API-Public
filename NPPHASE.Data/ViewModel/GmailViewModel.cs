namespace NPPHASE.Data.ViewModel
{
    public class GmailViewModel
    {
        public int? GmailId { get; set; }
        public string? ToEmail { get; set; }
        public string? FromEmail { get; set; }
        public string? BccEmail { get; set; }
        public string? CcEmail { get; set; }
        public string? Subject { get; set; }
        public string? MessageBody { get; set; }
        public string? MailType { get; set; }
        public DateTimeOffset? MailLogTime { get; set; }
        public int? DeviceUserId { get; set; }
    }
}
