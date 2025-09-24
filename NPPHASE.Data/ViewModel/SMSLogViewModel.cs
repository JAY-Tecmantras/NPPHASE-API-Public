using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class SMSLogViewModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string LogDate { get; set; }
        public string LogTime { get; set; }
        public string SmsType { get; set; }

    }
}
