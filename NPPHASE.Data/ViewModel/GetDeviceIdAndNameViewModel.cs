using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class GetDeviceIdAndNameViewModel
    {
        public int DeviceUserId { get; set; }
        public List<string> SearchField{ get; set; }        
    }
}