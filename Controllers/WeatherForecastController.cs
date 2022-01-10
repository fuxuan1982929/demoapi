using Microsoft.AspNetCore.Mvc;

namespace demoapi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching","a"
    };

    // private readonly ILogger<WeatherForecastController> _logger;

    // public WeatherForecastController(ILogger<WeatherForecastController> logger)
    // {
    //     _logger = logger;
    // }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {

        Console.WriteLine("RESP:GetWeatherForecast");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {   
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //comment it
            //aaa
        })
        .ToArray();
    }
}
