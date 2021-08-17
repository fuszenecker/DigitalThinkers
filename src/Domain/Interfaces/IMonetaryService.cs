using System.Collections.Generic;

namespace DigitalThinkers.Domain.Interfaces
{
    public interface IMonetaryService
    {
        void StoreNotes(IDictionary<uint, uint> notes);

        IDictionary<uint, uint> GetNotes();

        (string errorMessage, IDictionary<uint, uint> change) Checkout(IDictionary<uint, uint> notes, uint price);
    }
}