namespace NPPHASE.Data.ViewModel
{
    public class InstalledAppViewModel
    {
        public int? InstalledAppId { get; set; }
        public string? InstalledAppName { get; set; }
        public string? AppSize { get; set; }
        public int? DeviceUserId { get; set; }
        public DateTimeOffset? LogDateTime { get; set; }
    }
}
