using Microsoft.AspNetCore.Mvc;
using monitoring.Services;
using MQTTnet;
using MQTTnet.Client;

namespace monitoring.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly ILogger<MonitoringController> _logger;
    private readonly MonitoringService _monitoringService;

    public MonitoringController(ILogger<MonitoringController> logger, MonitoringService ms)
    {
        _logger = logger;
        _monitoringService = ms;
    }

    [Route("temp/{tempLimit}")]
    [HttpPost]
    public ActionResult Temperature(int tempLimit)
    {
        _monitoringService.Temp = tempLimit;
        return Ok();
    }

    [Route("humidity/{humidityLimit}")]
    [HttpPost]
    public ActionResult Humidity(int humidityLimit)
    {
        _monitoringService.Humidity = humidityLimit;
        return Ok();
    }

    [Route("co/{coLimit}")]
    [HttpPost]
    public ActionResult Co(int coLimit)
    {
        _monitoringService.Co = coLimit;
        return Ok();
    }

    [Route("smoke/{smokeLimit}")]
    [HttpPost]
    public ActionResult Smoke(int smokeLimit)
    {
        _monitoringService.Smoke = smokeLimit;
        return Ok();
    }

    [Route("lpg/{lpgLimit}")]
    [HttpPost]
    public ActionResult Lpg(int lpgLimit)
    {
        _monitoringService.Lpg = lpgLimit;
        return Ok();
    }
}
