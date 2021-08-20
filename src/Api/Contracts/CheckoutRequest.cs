using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalThinkers.Contracts
{
    public class CheckoutRequest
    {
        [Required]
        public CoinCollection Inserted { get; set; }

        // Should be decimal, but in the service we use uint for number of
        // coints and notes, and even for the value of coints and notes.
        [Required]
        public uint Price { get; set; }
    }
}