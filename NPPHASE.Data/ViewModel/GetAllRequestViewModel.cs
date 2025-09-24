namespace NPPHASE.Data.ViewModel
{
    public class GetAllRequestViewModel
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? DeviceUserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? ContactPersonName { get; set; }
    }
}
