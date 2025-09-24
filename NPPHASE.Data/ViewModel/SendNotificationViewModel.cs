using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class SendNotificationViewModel
    {
        public int DeviceUserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Key { get;set; }
        public string? Value { get;set; }

    }
}
