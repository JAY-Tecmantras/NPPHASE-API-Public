using Microsoft.AspNetCore.Http;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class ScreenShot : AuditabeEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Size { get; set; }
        public int? DeviceUserId { get; set; }

        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
