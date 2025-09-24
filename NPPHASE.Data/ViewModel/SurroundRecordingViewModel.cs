using Microsoft.AspNetCore.Http;
using NPPHASE.Data.Implementations;

namespace NPPHASE.Data.ViewModel
{
    public class SurroundRecordingViewModel 
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Duration { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile? File { get; set; }
        public int? DeviceUserId { get; set; }
        public DateTimeOffset? LogDateTime { get; set; }
    }
}
