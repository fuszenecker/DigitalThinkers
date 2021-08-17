using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.Domain.Services
{
    public class InMemoryMonetaryService : IMonetaryService
    {
        private Dictionary<uint, uint> store = new();
        private readonly object storeLocker = new();

        public void StoreNotes(IDictionary<uint, uint> notes)
        {
            lock (storeLocker)
            {
                foreach (var key in notes.Keys)
                {
                    if (store.ContainsKey(key))
                    {
                        store[key] += notes[key];
                    }
                    else
                    {
                        store[key] = notes[key];
                    }
                }
            }
        }

        public IDictionary<uint, uint> GetNotes()
        {
            lock (storeLocker)
            {
                // Better to return a copy of a store,
                // instead of providing a reference to it.
                return new Dictionary<uint, uint>(store);
            }
        }

        public (string errorMessage, IDictionary<uint, uint> change) Checkout(IDictionary<uint, uint> notes, uint price)
        {
            if (notes is null)
            {
                throw new ArgumentNullException(nameof(notes));
            }

            if (price == 0)
            {
                return ("Proce should not be zero.", null);
            }

            var total = notes.Sum(n => n.Key * n.Value);

            if (total < price)
            {
                return ($"More money should be inserted: {price - total} is missing, {total} as inserted.", null);
            }

            lock (storeLocker)
            {
                // The total amount of money we should give back.
                var change = total - price;

                // This store will contain all the coins and notes we should give back.
                // Hypotetically merge the current store and the money coming from customer.
                var newStore = new Dictionary<uint, uint>(store);

                foreach (var key in notes.Keys)
                {
                    if (store.ContainsKey(key))
                    {
                        newStore[key] += notes[key];
                    }
                    else
                    {
                        newStore[key] = notes[key];
                    }
                }

                // And calculate ehat to give back:
                var giveBack = new Dictionary<uint, uint>();

                foreach (var item in newStore.Keys.OrderByDescending(v => v))
                {
                    while (change >= item && newStore[item] > 0)
                    {
                        change -= item;
                        newStore[item]--;

                        if (giveBack.ContainsKey(item))
                        {
                            giveBack[item]++;
                        }
                        else
                        {
                            giveBack[item] = 1;
                        }
                    }
                }

                if (change == 0)
                {
                    // Commit changes:
                    store = newStore;
                    return (null, giveBack);
                }
                else
                {
                    return ($"Cannot accept money, {change} left.", null);
                }
            }
        }
    }
}