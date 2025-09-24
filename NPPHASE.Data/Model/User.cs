using Microsoft.AspNetCore.Identity;
using NPPHASE.Data.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class User : IdentityUser, IAuditabeEntity
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(20)]
        public string? CNIC { get; set; }
        [MaxLength(15)]
        public string? ContactNumber { get; set; }
        [MaxLength(500)]
        public string? PermanentAddress { get; set; }
        [MaxLength(500)]
        public string? PresentAddress { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset? ModificationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? PasswordNormal { get; set; }
    }
}
