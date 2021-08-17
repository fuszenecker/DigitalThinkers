using System;
using System.Collections.Generic;

namespace DigitalThinkers.Domain.Interfaces
{
    public interface INotesRepository
    {
        void StoreNotes(IDictionary<uint, uint> notes);

        void Transaction(Action action);

        IDictionary<uint, uint> GetNotes();
    }
}