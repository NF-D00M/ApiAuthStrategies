using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ApiAuthStrategies.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetWeather()
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();

            return Ok(forecast);
        }

        private record WeatherForecast(DateOnly Date, int TemperatureC, String? summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}