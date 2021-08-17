using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DigitalThinkers.Controllers
{
    public class RestControllerBase : ControllerBase
    {
        protected IEnumerable<string> GetNonNumericKeys(IDictionary<string, uint> collection)
        {
            return collection.Keys?.Where(k => !uint.TryParse(k, out var _));
        }

        protected Dictionary<uint, uint> MapNotes(IDictionary<string, uint> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var notes = new Dictionary<uint, uint>();

            foreach (var key in values.Keys)
            {
                notes.Add(uint.Parse(key), values[key]);
            }

            return notes;
        }
    }

}