using demoapi.Application.Commands;
using demoapi.Attributes;
using demoapi.MQ;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace demoapi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MQTestController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;
    public MQTestController(IMediator mediator,
                            ILogger<WeatherForecastController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    [MyApi]
    [HttpPut]
    public async Task<IActionResult> Send([FromBody] SendMQCommand command, [FromHeader(Name = "x-requestid")] string requestId)
    {
        bool commandResult = false;
        //_logger.LogInformation("发送消息：Test"); 
        if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            var identityCmd = new IdentifiedCommand<SendMQCommand, bool>(command, guid);

            commandResult = await _mediator.Send(identityCmd);
        }

        if (!commandResult)
        {
            return BadRequest();
        }

        return Ok();

    }
}
