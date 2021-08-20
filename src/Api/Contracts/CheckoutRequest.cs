using System.Collections.Generic;

namespace DigitalThinkers.Contracts
{
    public class CheckoutRequest
    {
        public CoinCollection Inserted { get; set; }

        // Should be decimal, but in the service we use uint for number of
        // coints and notes, and even for the value of coints and notes.
        public uint Price { get; set; }
    }
}