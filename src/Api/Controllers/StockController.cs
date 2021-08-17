using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalThinkers.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> logger;
        private readonly IMoneyStore moneyStore;

        public StockController(ILogger<StockController> logger, IMoneyStore moneyStore)
        {
            this.logger = logger;
            this.moneyStore = moneyStore;
        }

        [HttpPost]
        public IActionResult Post([FromBody]IDictionary<string, uint> request)
        {
            if (request is null)
            {
                const string message = "Request body is empty";
                this.logger.LogWarning(message);
                return BadRequest(message);
            }

            IEnumerable<string> nonNumericKeys = request.Keys.Where(k => !Int32.TryParse(k, out var _));

            if (nonNumericKeys.Any())
            {
                this.logger.LogWarning("Keys are not numeric: {keys}", nonNumericKeys);
                return BadRequest($"Keys {string.Join(',', nonNumericKeys)} are not numbers.");
            }

            var notes = new Dictionary<uint, uint>();

            foreach (var key in request.Keys)
            {
                notes.Add(uint.Parse(key), request[key]);
            }

            this.logger.LogDebug("Storing notes: {@notes}", notes);

            moneyStore.StoreNotes(notes);

            return Ok(moneyStore.GetNotes());
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(moneyStore.GetNotes());
        }
    }
}
