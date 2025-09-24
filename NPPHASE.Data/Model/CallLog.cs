using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class CallLog : AuditabeEntity
    {
        public int CallLogId { get; set; }
        public int? DeviceUserId { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(15)]
        public string? Number { get; set; }
        public IncomingOutgoingTypes CallTypes { get; set; }
        public string? CallDuration { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Recording { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset LogDateTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;

    }
}
