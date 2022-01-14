using demoapi.Attributes;
using demoapi.MQ;
using Microsoft.AspNetCore.Mvc;

namespace demoapi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MQTestController  : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly RabbitMQHelper _mQHelpler;
    public MQTestController(ILogger<WeatherForecastController> logger, 
                            RabbitMQHelper mQHelpler)
    {
        _logger = logger;
        _mQHelpler = mQHelpler;
    }

    [MyApi]
    [HttpGet()]
    public bool Send()
    {
        _logger.LogInformation("发送消息：Test");
        return _mQHelpler.SendMQ<string>("queue1","Test");
    }
}
