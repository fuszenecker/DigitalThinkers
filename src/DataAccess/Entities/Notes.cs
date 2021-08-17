using System.ComponentModel.DataAnnotations;

namespace DigitalThinkers.DataAccess.Entities
{
    public class Note
    {
        [Key]
        public uint Denominator { get; set; }
        public uint Count { get; set; }
    }
}