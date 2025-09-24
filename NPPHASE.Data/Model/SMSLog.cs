using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class SMSLog : AuditabeEntity
    {
        public int SMSLogId { get; set; }
        public int? DeviceUserId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(15)]
        public string Number { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        public SmsType SmsType { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;



    }
}
