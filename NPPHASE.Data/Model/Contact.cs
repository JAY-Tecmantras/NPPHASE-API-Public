using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class Contact : AuditabeEntity
    {
        public int ContactId { get; set; }
        public int DeviceUserId { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(15)]
        public string? MobileNumber { get; set; }
        [MaxLength(15)]
        public string? HomeNumber { get; set; }
        [MaxLength(15)]
        public string? OfficeNumber { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }

        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
