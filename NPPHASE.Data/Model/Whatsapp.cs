using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Whatsapp : AuditabeEntity
    {
        public int WhatsappId { get; set; }
        [MaxLength(20)]
        public string? ContactNumber { get; set; }
        [MaxLength(500)]
        public string? ContactPersonName { get; set; }
        public IncomingOutgoingTypes? MessageType { get; set; }
        public string? Message { get; set; }
        public int? DeviceUserId { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset? MessageLogTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
