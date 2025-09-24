using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Gmail : AuditabeEntity
    {
        public int GmailId { get; set; }
        [MaxLength(50)]
        public string? ToEmail { get; set; }
        [MaxLength(50)]
        public string? FromEmail { get; set; }
        [MaxLength(50)]
        public string? BccEmail { get; set; }
        [MaxLength(50)]
        public string? CcEmail { get; set; }
        [MaxLength(200)]
        public string? Subject { get; set; }
        public string? MessageBody { get; set; }
        public IncomingOutgoingTypes? MailType { get; set; }
        public DateTimeOffset? MailLogTime { get; set; }
        public int? DeviceUserId { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
