using Microsoft.AspNetCore.Http;
using NPPHASE.Data.Enum;


namespace NPPHASE.Data.ViewModel
{
    public class CallLogViewModel
    {
        public int CallLogId { get; set; }
        public int? DeviceUserId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string CallTypes { get; set; }
        public string CallDuration { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Recording { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
    }

    public class AddCallLogViewModel
    {
        public int? DeviceUserId { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }
        public IncomingOutgoingTypes? CallTypes { get; set; }
        public string? CallDuration { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Recording { get; set; }
        public DateTimeOffset? LogDateTime { get; set; }
        public IFormFile? AudioFile { get; set; }
    }
}
