using System;
using DigitalThinkers.Domain.Entities;

namespace DigitalThinkers.Domain.Interfaces
{
    public interface ICoinsRepository
    {
        void StoreCoins(CoinCollection coins);

        void Transaction(Action action);

        CoinCollection GetCoins();
    }
}