using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class InternetHistory : AuditabeEntity
    {
        public int InternetHistoryId { get; set; }
        public string? WebUrl { get; set; }
        public string? WebLogTime { get; set; }
        public int? DeviceUserId { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
