using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class WiFiNetworksViewModel
    {
        public int? WiFINetworkId { get; set; }
        public string? WiFINetworkName { get; set; }
        public bool? IsProtecteted { get; set; }
        public string? Strength { get; set; }
        public int? DeviceUserId { get; set; }
        public DateTimeOffset CreationDate { get; set; }

    }
}
