using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class AnalyticsViewModel
    {
        public int ActiveUserCount { get; set; }   
        public int InActiveUserCount { get; set; }   
        public int AndroidCount { get; set; }   
        public int IphoneCount { get; set; }   
        public int NewMemberCount { get; set; }   
        public int TotalPhoneCount { get; set; }
        public List<MonthCount> ActiveUserMonthlyCount { get; set; }
        public List<MonthCount> InActiveUserMonthlyCount { get; set; }
        public List<MonthCount> NewMemberMonthlyCount { get; set; }
    }
    public class MonthCount
    {
        public int Count { get; set; }
        public string Month { get;set; }
    }
}
