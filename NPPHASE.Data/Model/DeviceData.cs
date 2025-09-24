using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NPPHASE.Data.Enum;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class DeviceData : AuditabeEntity
    {
        public int DeviceDataId { get; set; }
        public int? DeviceUserId { get; set; }
        public bool? IsConnectedWithWifi { get; set; }
        public decimal? BatteryPercentage { get; set; }
        public DeviceStatus? Status { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
