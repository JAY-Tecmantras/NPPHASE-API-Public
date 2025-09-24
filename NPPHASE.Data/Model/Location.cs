using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Location : AuditabeEntity
    {
        public int LocationId { get; set; }
        public int DeviceUserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset LogDateTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
