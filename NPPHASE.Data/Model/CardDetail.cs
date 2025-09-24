using NPPHASE.Data.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Model
{
    public class CardDetail : AuditabeEntity
    {
        public int CardDetailId { get; set; }
        public int DeviceUserId { get; set; }
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ExpiryDate { get; set; }
        public int CVV { get; set; }
        [ForeignKey("DeviceUserId")]
        public virtual DeviceUser? DeviceUser { get; set; } = null!;
    }
}
