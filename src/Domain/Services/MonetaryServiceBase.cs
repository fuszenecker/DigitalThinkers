using System.Collections.Generic;
using System.Linq;

namespace DigitalThinkers.Domain.Services
{
    public class MonetaryServiceBase
    {
        protected static void MergeNotes(IDictionary<uint, uint> notes, IDictionary<uint, uint> store)
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

        protected static (uint, IDictionary<uint, uint>) PayBack (uint change, IDictionary<uint, uint> newStore)
        {
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

            return (change, giveBack);
       }
    }
}