using System;
using ServiceTemplate.Domain.Entities;

namespace ServiceTemplate.Domain.Interfaces
{
    public interface ICoinsRepository
    {
        void StoreCoins(CoinCollection coins);

        void Transaction(Action action);

        CoinCollection GetCoins();
    }
}