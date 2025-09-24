﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class TokenRequestViewModel
    {
        public string JwtKey { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? DeviceId { get; set; }

    }
}
