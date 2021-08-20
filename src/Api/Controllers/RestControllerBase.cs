using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DigitalThinkers.Controllers
{
    public class RestControllerBase : ControllerBase
    {
        protected IEnumerable<string> GetNonNumericKeys(Contracts.CoinCollection collection)
        {
            return collection.Keys.Where(k => !uint.TryParse(k, out var _));
        }

        protected Domain.Entities.CoinCollection MapCoins(Contracts.CoinCollection coins)
        {
            if (coins == null)
            {
                throw new ArgumentNullException(nameof(coins));
            }

            var mappedCoins = new Domain.Entities.CoinCollection();

            foreach (var key in coins.Keys)
            {
                mappedCoins.Add(uint.Parse(key), coins[key]);
            }

            return mappedCoins;
        }
    }
}