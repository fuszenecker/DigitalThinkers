// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace DigitalThinkers.Controllers
// {
//     [ApiController]
//     [Route("api/v{version:apiVersion}/[controller]")]
//     [ApiVersion("1.0")]
//     public class BlockedBillsController : ControllerBase
//     {
//         private static readonly string[] Summaries = new[]
//         {
//             "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//         };

//         private readonly ILogger<BlockedBillsController> _logger;

//         public BlockedBillsController(ILogger<BlockedBillsController> logger)
//         {
//             _logger = logger;
//         }

//         [HttpGet]
//         public IEnumerable<WeatherForecast> Get()
//         {
//             var rng = new Random();
//             return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//             {
//                 Date = DateTime.Now.AddDays(index),
//                 TemperatureC = rng.Next(-20, 55),
//                 Summary = Summaries[rng.Next(Summaries.Length)]
//             })
//             .ToArray();
//         }
//     }
// }
