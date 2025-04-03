using Microsoft.AspNetCore.Mvc;
using pattern.sample.api.StrategyHandler;
using pattern.proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.sample.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IProxyContext StrategyContext { get; }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(IProxyContext strategyContext)
        {
            StrategyContext = strategyContext;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
        {
            var response = await StrategyContext.HandlerAsync<TestStrategyRequest, TestStrategyResponse>(new TestStrategyRequest() { Name = "Teste123" }, cancellationToken);
            var rng = new Random();
            var response1 = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return response1;
        }
    }
}
