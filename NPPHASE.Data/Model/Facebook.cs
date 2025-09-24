using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Data.Model
{
    public class Facebook : AuditabeEntity
    {
        public int FacebookId { get; set; }
        [MaxLength(20)]
        public string? ContactNumber { get; set; }
        [MaxLength(500)]
        public string? ContactPersonName { get; set; }
        [MaxLength(50)]
        public string? UserName { get; set; }
        public IncomingOutgoingTypes? MessageType { get; set; }
        public string? Message { get; set; }
        public int? DeviceUserId { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset? MessageLogTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }

}
