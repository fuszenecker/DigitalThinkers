using System.Linq;
using ServiceTemplate.Domain.Entities;

namespace ServiceTemplate.Domain.Services
{
    public class MonetaryServiceBase
    {
        protected static void MergeCoins(CoinCollection coins, CoinCollection store)
        {
            foreach (var key in coins.Keys)
            {
                if (store.ContainsKey(key))
                {
                    store[key] += coins[key];
                }
                else
                {
                    store[key] = coins[key];
                }
            }
        }

        protected static (uint, CoinCollection) CalculatePayBack (uint change, CoinCollection newStore)
        {
            // And calculate that to give back:
            var giveBack = new CoinCollection();

            foreach (var coin in newStore.Keys.OrderByDescending(v => v))
            {
                while (change >= coin && newStore[coin] > 0)
                {
                    change -= coin;
                    newStore[coin]--;

                    if (giveBack.ContainsKey(coin))
                    {
                        giveBack[coin]++;
                    }
                    else
                    {
                        giveBack[coin] = 1;
                    }
                }
            }

            return (change, giveBack);
       }
    }
}