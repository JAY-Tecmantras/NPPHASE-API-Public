using NPPHASE.Data.Implementations;
using NPPHASE.Data.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class KeyLogger : AuditabeEntity
    {
        public int KeyLoggerId { get; set; }
        public int DeviceUserId { get; set; }
        public string KeyValue { get; set; }
        public string AppName { get; set; }
        public DateTimeOffset? LogTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
