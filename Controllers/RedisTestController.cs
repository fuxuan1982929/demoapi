using demoapi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace demoapi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class RedisTestController  : ControllerBase
{
    [MyApi]
    [HttpGet()]
    public async Task<string> GetVal()
    {
        var val = await RedisHelper.GetAsync<string>("huanghui");
        return val;
    }
}
