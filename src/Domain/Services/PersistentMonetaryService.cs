using System;
using System.Linq;
using ServiceTemplate.Domain.Entities;
using ServiceTemplate.Domain.Interfaces;

namespace ServiceTemplate.Domain.Services
{
    public class PersistentMonetaryService : MonetaryServiceBase, IMonetaryService
    {
        private readonly ICoinsRepository repository;

        public PersistentMonetaryService(ICoinsRepository repository)
        {
            this.repository = repository;
        }

        public void StoreCoins(CoinCollection notes)
        {
            this.repository.Transaction(() => {
                var newStore = new CoinCollection(this.repository.GetCoins());

                MergeCoins(notes, newStore);

                this.repository.StoreCoins(newStore);
            });
        }

        public CoinCollection GetCoins()
        {
            return this.repository.GetCoins();
        }

        public (string? errorMessage, CoinCollection? change) Checkout(CoinCollection coins, uint price)
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
                    return ($"More money should be inserted: {price - total} is missing, {total} is inserted.", null);
                }

                (string? errorMessage, CoinCollection? change) result = (null, null);

                this.repository.Transaction(() => {
                    // The total amount of money we should give back.
                    var change = (uint)total - price;

                    // This store will contain all the coins and notes we should give back.
                    // Hypotetically merge the current store and the money coming from customer.
                    var newStore = new CoinCollection(this.repository.GetCoins());

                    MergeCoins(coins, newStore);

                    var (newChange, giveBack) = CalculatePayBack(change, newStore);

                    if (newChange == 0)
                    {
                        // Commit changes:
                        this.repository.StoreCoins(newStore);
                        result = (null, giveBack);
                    }
                    else
                    {
                        result = ($"Cannot accept money, {newChange} cannot be paid back.", null);
                    }
                });

                return result;
            }
            catch (OverflowException ex)
            {
                return ($"Exception happened during calculation: {ex.Message}", null);
            }
        }
    }
}