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
    // [ApiVersion("2")]
    public class CheckoutController : RestControllerBase
    {
        private readonly ILogger<CheckoutController> logger;
        private readonly IMonetaryService monetaryService;

        public CheckoutController(ILogger<CheckoutController> logger, IMonetaryService monetaryService)
        {
            this.logger = logger;
            this.monetaryService = monetaryService;
        }

        /// <summary>
        /// Checkout: tell the price and insert coins to the machine.
        /// </summary>
        /// <param name="request">The DTO for coins to be inserted.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Dictionary<string, uint>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody]CheckoutRequest request)
        {
            if (request is null)
            {
                const string message = "Request body is empty.";
                this.logger.LogWarning(message);
                return BadRequest(message);
            }

            if (request.Inserted is null)
            {
                const string message = "List of inserted coins is empty.";
                this.logger.LogWarning(message);
                return BadRequest(message);
            }

            IEnumerable<string> nonNumericKeys = GetNonNumericKeys(request.Inserted);

            if (nonNumericKeys.Any())
            {
                this.logger.LogWarning("Keys are not numeric: {keys}.", nonNumericKeys);
                return BadRequest($"Keys {string.Join(',', nonNumericKeys)} are not numbers.");
            }

            if (request.Price == 0)
            {
                const string message = "Price should be non-zero.";
                this.logger.LogWarning(message);
                return BadRequest(message);
            }

            var coins = MapCoins(request.Inserted);

            this.logger.LogDebug("Trying to pay {price} with {@coins}.", request.Price, coins);

            var (errorMessage, change) = monetaryService.Checkout(coins, request.Price);

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                this.logger.LogInformation("Successfully paid {price}.", request.Price);
                return Ok(change);
            }
            else
            {
                this.logger.LogWarning("Error happened during payment of {price}: {message}.",
                    request.Price, errorMessage);

                return BadRequest(errorMessage);
            }
        }
    }
}
