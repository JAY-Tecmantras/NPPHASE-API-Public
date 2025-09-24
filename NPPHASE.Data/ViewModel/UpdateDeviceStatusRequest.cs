using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class UpdateDeviceStatus 
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DeviceUserId must be greater than 0.")]
        public int DeviceUserId { get; set; } 
        [Required]
        public DeviceStatus? Status { get; set; }
    }
}
