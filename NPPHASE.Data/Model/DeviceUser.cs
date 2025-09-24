using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class DeviceUser : AuditabeEntity
    {
        public int DeviceUserId { get; set; }
        [MaxLength(6)]
        public string? DeviceUniqueId { get; set; }
        [Required]
        [MaxLength(50)]
        public string DeviceName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Model { get; set; }
        [Required]
        [MaxLength(20)]
        public string OS { get; set; }
        [Required]
        [MaxLength(50)]
        public string Version { get; set; }
        [Required]
        [MaxLength(20)]
        public string IMEINumber { get; set; }
        public string? UserId { get; set; }
        [MaxLength(512)]
        public string? DeviceToken { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; } = null!;
        public int AlacExtractionProgress {get; set; }
        public int AlacAllotmentProgress {get; set;}
        public int PrivateKeyExtractionProgress {get; set; }
        public int AlacDecryptionProgress { get; set; }
        public int PhaseMonitor { get; set; }
    }
}
