using System.Collections.Generic;

namespace ServiceTemplate.Domain.Entities
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