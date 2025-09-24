using System.ComponentModel.DataAnnotations;

namespace NPPHASE.Data.ViewModel
{
    public class AddProfileRequestViewModel
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? CNIC { get; set; }

        [MaxLength(15)]
        public string? ContactNumber { get; set; }

        [MaxLength(500)]
        public string? PermanentAddress { get; set; }

        [MaxLength(500)]
        public string? PresentAddress { get; set; }
    }
}
