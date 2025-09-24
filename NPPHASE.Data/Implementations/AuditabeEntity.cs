using NPPHASE.Data.Interface;
using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Data.Implementations
{
    public class AuditabeEntity : IAuditabeEntity
    {
        [ScaffoldColumn(false)]
        public DateTimeOffset CreationDate { get; set; }


        [ScaffoldColumn(false)]
        public string? CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTimeOffset? ModificationDate { get; set; }


        [ScaffoldColumn(false)]
        public string? ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
