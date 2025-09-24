using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class ScreenTimeViewModel
    {
        public int? ScreenTimeId { get; set; }
        public string? AppName { get; set; }
        public string? ScreenTimeDuration { get; set; }
        public int? DeviceUserId { get; set; }
        //public string? TotalScreenTime { get; set; }
        //public string? TopApplicationName { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
    public class ScreenTimeAppNameTotalTimeViewModel
    {
        public List<string?> AppName { get; set; }
        public string? TotalTime { get; set; }
        public string? TotalScreenTime { get; set; }
    }
    public class AppNameScreenTimeViewModel
    {
        public string? AppName { get; set; }
        public long? ScreenTimeDuration { get; set; }
    }
}
