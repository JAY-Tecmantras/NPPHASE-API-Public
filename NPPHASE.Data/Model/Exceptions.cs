using System.ComponentModel.DataAnnotations.Schema;

namespace NPPHASE.Data.Model
{
    public class Exceptions
    {
        public int ExceptionsId { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? ScreenName { get; set; }
        [Column(TypeName = "timestamp(6)")]
        public DateTimeOffset Timestamp { get; set; }
    }
}
