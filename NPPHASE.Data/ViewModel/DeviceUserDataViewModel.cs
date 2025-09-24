using Microsoft.AspNetCore.Identity;
using NPPHASE.Data.Enum;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class DeviceUserDataViewModel
    {
        // User        
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        // Device User       
        [Required]
        [MaxLength(50)]
        public string DeviceName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Model { get; set; }
        [Required]
        [MaxLength(20)]
        public string OS { get; set; }
        [Required]
        [MaxLength(50)]
        public string Version { get; set; }              
        [Required]
        [MaxLength(20)]
        public string IMEINumber { get; set; }

        // Device Data       
        public bool? IsConnectedWithWifi { get; set; }
        public decimal? BatteryPercentage { get; set; }            

    }

}
