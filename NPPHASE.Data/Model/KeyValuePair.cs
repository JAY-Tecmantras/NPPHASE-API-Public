using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class KeyValuePair : AuditabeEntity
    {
        public int KeyValuePairId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
