using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class ScreenTime : AuditabeEntity
    {
        public int ScreenTimeId { get; set; }
        public string? AppName { get; set; }
        public string? ScreenTimeDuration { get; set; }
        public int? DeviceUserId { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; }
    }
}
