using System.ComponentModel.DataAnnotations;

namespace ServiceTemplate.DataAccess.Entities
{
    public class CoinCount
    {
        [Key]
        public uint Denominator { get; set; }
        public uint Count { get; set; }
    }
}