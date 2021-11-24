using System;
using System.Linq;
using ServiceTemplate.Domain.Entities;
using ServiceTemplate.Domain.Interfaces;

namespace ServiceTemplate.Domain.Services
{
    public class InMemoryMonetaryService : MonetaryServiceBase, IMonetaryService
    {
        private CoinCollection store = new();
        private readonly object storeLocker = new();

        public void StoreCoins(CoinCollection coins)
        {
            lock (storeLocker)
            {
                MergeCoins(coins, store);
            }
        }

        public CoinCollection GetCoins()
        {
            lock (storeLocker)
            {
                // Better to return a copy of a store,
                // instead of providing a reference to it.
                return new CoinCollection(store);
            }
        }

        public (string errorMessage, CoinCollection change) Checkout(CoinCollection coins, uint price)
        {
            if (coins is null)
            {
                throw new ArgumentNullException(nameof(coins));
            }

            if (price == 0)
            {
                return ("Proce should not be zero.", null);
            }

            try
            {
                var total = coins.Sum(n => n.Key * n.Value);

                if (total < price)
                {
                    return ($"More money should be inserted: {price - total} is missing, {total} as inserted.", null);
                }

                lock (storeLocker)
                {
                    // The total amount of money we should give back.
                    var change = (uint)total - price;

                    // This store will contain all the coins and notes we should give back.
                    // Hypotetically merge the current store and the money coming from customer.
                    var newStore = new CoinCollection(store);

                    MergeCoins(coins, newStore);

                    var (newChange, giveBack) = CalculatePayBack(change, newStore);

                    if (newChange == 0)
                    {
                        // Commit changes:
                        store = newStore;
                        return (null, giveBack);
                    }
                    else
                    {
                        return ($"Cannot accept money, {newChange} cannot be paid back.", null);
                    }
                }
            }
            catch (OverflowException ex)
            {
                return ($"Exception happened during calculation: {ex.Message}", null);
            }
        }
    }
}