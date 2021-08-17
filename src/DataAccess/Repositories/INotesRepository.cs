using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.DataAccess.Contexts;
using DigitalThinkers.Domain.Interfaces;

namespace DigitalThinkers.DataAccess.Repositories
{
    public class NotesRepository : INotesRepository
    {
        public IDictionary<uint, uint> GetNotes()
        {
            using var context = new NotesContext();
            var result = new Dictionary<uint, uint>();

            foreach (var note in context.Notes)
            {
                result[note.Denominator] = note.Count;
            }

            return result;
        }

        public void StoreNotes(IDictionary<uint, uint> notes)
        {
            throw new NotImplementedException();
        }

        public void Transaction(Action action)
        {
            throw new NotImplementedException();
        }
    }
}