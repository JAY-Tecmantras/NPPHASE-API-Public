using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class AdminLoginUserDetailsChangeViewModel
    {      
        [Required]
        public string NewUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
