using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class WiFiNetwork : AuditabeEntity
    {
        public int WiFINetworkId { get; set; }
        public string? WiFINetworkName { get; set; }
        public bool? IsProtecteted { get; set; } = true;
        public string? Strength { get; set; }
        public int? DeviceUserId { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
