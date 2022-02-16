using System;
using ServiceTemplate.DataAccess.Contexts;
using ServiceTemplate.DataAccess.Entities;
using ServiceTemplate.Domain.Entities;
using ServiceTemplate.Domain.Interfaces;

namespace ServiceTemplate.DataAccess.Repositories
{
    public class CoinsRepository : ICoinsRepository, IDisposable
    {
        private CoinsContext? context = new();

        public CoinsRepository()
        {
            this.context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            if (context is not null)
            {
                context.Dispose();
                GC.SuppressFinalize(this);
                context = null;
            }
        }

        public CoinCollection GetCoins()
        {
            var result = new CoinCollection();

            if (context is not null)
            {
                foreach (var coin in context.Coins)
                {
                    result[coin.Denominator] = coin.Count;
                }
            }

            return result;
        }

        public void StoreCoins(CoinCollection coins)
        {
            if (context is not null)
            {
                // Delete all existing items, and add the new items,
                foreach (var item in context.Coins)
                {
                    context.Coins.Remove(item);
                }
            }

            foreach (var coin in coins)
            {
                context?.Coins.Add(new CoinCount()
                {
                    Denominator = coin.Key,
                    Count = coin.Value
                });
            }

            context?.SaveChanges();
        }

        public void Transaction(Action action)
        {
            using var transaction = context?.Database.BeginTransaction();
            action();
            transaction?.Commit();
        }
    }
}