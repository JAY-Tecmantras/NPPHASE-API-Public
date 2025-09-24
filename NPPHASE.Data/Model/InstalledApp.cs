using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class InstalledApp : AuditabeEntity
    {
        public int InstalledAppId { get; set; }
        public string? InstalledAppName { get; set; }
        public string? AppSize { get; set; }
        public int? DeviceUserId { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset? LogDateTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
