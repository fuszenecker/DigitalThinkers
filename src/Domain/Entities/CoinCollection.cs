using System.Collections.Generic;

namespace DigitalThinkers.Domain.Entities
{
    public class CoinCollection : Dictionary<uint, uint>
    {
        public CoinCollection()
        {
        }

        public CoinCollection(CoinCollection coins) : base(coins)
        {
        }
    }
}