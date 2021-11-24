using System.Collections.Generic;
using System.Linq;
using System.Net;
using ServiceTemplate.Contracts;
using ServiceTemplate.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceTemplate.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class StockController : RestControllerBase
    {
        private readonly ILogger<StockController> logger;
        private readonly IMonetaryService monetaryService;

        public StockController(ILogger<StockController> logger, IMonetaryService monetaryService)
        {
            this.logger = logger;
            this.monetaryService = monetaryService;
        }

        /// <summary>
        /// Insert additional coins to the machine.
        /// </summary>
        /// <param name="request">The DTO for coins to be inserted.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Dictionary<string, uint>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody]CoinCollection request)
        {
            if (request is null)
            {
                const string message = "Request body is empty";
                this.logger.LogWarning(message);
                return BadRequest(message);
            }

            IEnumerable<string> nonNumericKeys = GetNonNumericKeys(request);

            if (nonNumericKeys.Any())
            {
                this.logger.LogWarning("Keys are not numeric: {keys}", nonNumericKeys);
                return BadRequest($"Keys {string.Join(',', nonNumericKeys)} are not numbers.");
            }

            var coins = MapCoins(request);
            this.logger.LogDebug("Storing coins: {@coins}", coins);
            monetaryService.StoreCoins(coins);

            return Ok(monetaryService.GetCoins());
        }

        /// <summary>
        /// Gets the coin state from the machine.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(CoinCollection), (int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return Ok(monetaryService.GetCoins());
        }
    }
}
