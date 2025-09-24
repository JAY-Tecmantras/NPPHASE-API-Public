using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Data.ViewModel
{
    public class DeviceUserViewModel
    {
        public int? DeviceUserId { get; set; }
        public int? DeviceUniqueId { get; set; }
        public string? DeviceName { get; set; }

        public string? Model { get; set; }

        public string? OS { get; set; }
        public string? Version { get; set; }

        public string? IMEINumber { get; set; }
        public string? UserId { get; set; }
        public string? BatteryPerc { get; set; }
        public bool? IsConnectedWithWifi { get; set; }
        public string? DeviceStatus { get; set; }
        public int AlacExtractionProgress { get; set; }
        public int AlacAllotmentProgress { get; set; }
        public int PrivateKeyExtractionProgress { get; set; }
        public int AlacDecryptionProgress { get; set; }
        public int PhaseMonitor { get; set; }
    }  
}
