namespace NPPHASE.Data.ViewModel
{
    public class LineViewModel
    {
        public int? LineId { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactPersonName { get; set; }
        public string? MessageType { get; set; }
        public string? Message { get; set; }
        public int? DeviceUserId { get; set; }
        public DateTimeOffset? MessageLogTime { get; set; }
    }
}
