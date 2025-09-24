using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class SurroundRecordings : AuditabeEntity
    {
        public int Id { get; set; }
        public string? RecordingName { get; set; }
        public string? RecordingDuration { get; set; }
        public int? DeviceUserId { get; set; }

        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
