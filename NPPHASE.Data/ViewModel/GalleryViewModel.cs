using NPPHASE.Data.Implementations;

namespace NPPHASE.Data.ViewModel
{
    public class GalleryViewModel : AuditabeEntity
    {
        public int GalleryId { get; set; }
        public int DeviceUserId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string FileTypes { get; set; }
        public string FileUrl { get; set; }
        public DateTimeOffset LogDateTime { get; set; }
    }
}
