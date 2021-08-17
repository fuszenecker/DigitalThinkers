using System.Collections.Generic;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.Domain.Services
{
    public class MoneyStore : IMoneyStore
    {
        private readonly Dictionary<uint, uint> store = new Dictionary<uint, uint>();

        public void StoreNotes(Dictionary<uint, uint> notes)
        {
            lock (store)
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

        public Dictionary<uint, uint> GetNotes()
        {
            lock (store)
            {
                // Better to return a copy of a store,
                // instead of providing a reference to it.
                return new Dictionary<uint, uint>(store);
            }
        }
    }
}