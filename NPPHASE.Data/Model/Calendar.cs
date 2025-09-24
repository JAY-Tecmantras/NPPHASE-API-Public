using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Calendar : AuditabeEntity
    {
        public int CalendarId { get; set; }
        public int? DeviceUserId { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(200)]
        public string? Time { get; set; }
        public DateTimeOffset? CalenderLogTime { get; set; }

        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
