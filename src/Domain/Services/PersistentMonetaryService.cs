using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.Domain.Services
{
    public class PersistentMonetaryService : IMonetaryService
    {
        private readonly INotesRepository repository;

        public PersistentMonetaryService(INotesRepository repository)
        {
            this.repository = repository;
        }

        public void StoreNotes(IDictionary<uint, uint> notes)
        {
            this.repository.Transaction(() => {
                var newStore = new Dictionary<uint, uint>(this.repository.GetNotes());

                foreach (var key in notes.Keys)
                {
                    if (newStore.ContainsKey(key))
                    {
                        newStore[key] += notes[key];
                    }
                    else
                    {
                        newStore[key] = notes[key];
                    }
                }

                this.repository.StoreNotes(newStore);
            });
        }

        public IDictionary<uint, uint> GetNotes()
        {
            return this.repository.GetNotes();
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
                return ($"More money should be inserted: {price - total} is missing, {total} is inserted.", null);
            }

            (string errorMessage, IDictionary<uint, uint> change) result = (null, null);

            this.repository.Transaction(() => {
                // The total amount of money we should give back.
                var change = total - price;

                // This store will contain all the coins and notes we should give back.
                // Hypotetically merge the current store and the money coming from customer.
                var newStore = new Dictionary<uint, uint>(this.repository.GetNotes());

                foreach (var key in notes.Keys)
                {
                    if (newStore.ContainsKey(key))
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
                    this.repository.StoreNotes(newStore);
                    result = (null, giveBack);
                }
                else
                {
                    result = ($"Cannot accept money, {change} cannot be paid back.", null);
                }
            });

            return result;
        }
    }
}