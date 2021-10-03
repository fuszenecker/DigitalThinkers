using DigitalThinkers.Domain.Entities;

namespace DigitalThinkers.Domain.Interfaces
{
    public interface IMonetaryService
    {
        void StoreCoins(CoinCollection coins);

        CoinCollection GetCoins();

        (string errorMessage, CoinCollection change) Checkout(CoinCollection coins, uint price);
    }
}