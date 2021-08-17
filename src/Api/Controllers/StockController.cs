using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalThinkers.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class StockController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<StockController> logger;

        public StockController(ILogger<StockController> logger)
        {
            this.logger = logger;
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

            return Ok(null);
        }
    }
}
