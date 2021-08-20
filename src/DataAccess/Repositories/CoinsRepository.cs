using System;
using System.Collections.Generic;
using DigitalThinkers.DataAccess.Contexts;
using DigitalThinkers.DataAccess.Entities;
using DigitalThinkers.Domain.Entities;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.DataAccess.Repositories
{
    public class CoinsRepository : ICoinsRepository, IDisposable
    {
        private CoinsContext context = new();

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

            foreach (var coin in context.Coins)
            {
                result[coin.Denominator] = coin.Count;
            }

            return result;
        }

        public void StoreCoins(CoinCollection coins)
        {
            // Delete all existing items, and add the new items,
            foreach (var item in context.Coins)
            {
                context.Coins.Remove(item);
            }

            foreach (var coin in coins)
            {
                context.Coins.Add(new Coin()
                {
                    Denominator = coin.Key,
                    Count = coin.Value
                });
            }

            context.SaveChanges();
        }

        public void Transaction(Action action)
        {
            using var transaction = context.Database.BeginTransaction();
            action();
            transaction.Commit();
        }
    }
}