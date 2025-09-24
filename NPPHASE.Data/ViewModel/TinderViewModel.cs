namespace NPPHASE.Data.ViewModel
{
    public class TinderViewModel
    {
        public int? TinderId { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPersonName { get; set; }
        public string? UserName { get; set; }
        public string MessageType { get; set; }
        public string? Message { get; set; }
        public int? DeviceUserId { get; set; }
        public DateTimeOffset? MessageLogTime { get; set; }
    }
}
