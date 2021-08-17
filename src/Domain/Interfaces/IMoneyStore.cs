using System.Collections.Generic;

namespace DigitalThinkers.Domain.Interfaces
{
    public interface IMoneyStore
    {
        void StoreNotes(Dictionary<uint, uint> notes);

        Dictionary<uint, uint> GetNotes();
    }
}