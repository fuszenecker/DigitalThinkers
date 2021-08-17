using System;
using System.Collections.Generic;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.Domain.Services
{
    public class MonesStore : IMoneyStore
    {
        public void StoreNotes(Dictionary<uint, uint> notes)
        {

        }

        public Dictionary<uint, uint> GetNotes()
        {
            throw new NotImplementedException();
        }
    }
}