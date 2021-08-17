using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Contracts;
using DigitalThinkers.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalThinkers.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CheckoutController : RestControllerBase
    {
        private readonly ILogger<CheckoutController> logger;
        private readonly IMonetaryService monetaryService;

        public CheckoutController(ILogger<CheckoutController> logger, IMonetaryService monetaryService)
        {
            this.logger = logger;
            this.monetaryService = monetaryService;
        }

        [HttpPost]
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

            var notes = MapNotes(request.Inserted);

            this.logger.LogDebug("Trying to pay {price} with {@notes}.", request.Price, notes);

            var (errorMessage, change) = monetaryService.Checkout(notes, request.Price);

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                this.logger.LogInformation("Successfully payd {proce}.", request.Price);
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
