using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Gallery : AuditabeEntity
    {
        public int GalleryId { get; set; }
        public int DeviceUserId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public FileType FileTypes { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
