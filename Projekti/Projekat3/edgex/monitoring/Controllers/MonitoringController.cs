using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using monitoring.Services;
using MQTTnet;
using MQTTnet.Client;

namespace monitoring.Controllers
{

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
        public ActionResult Temperature(double tempLimit)
        {
            _monitoringService.limits["temp"] = tempLimit;
            return Ok();
        }

        [Route("humidity/{humidityLimit}")]
        [HttpPost]
        public ActionResult Humidity(double humidityLimit)
        {
            _monitoringService.limits["humidity"] = humidityLimit;
            return Ok();
        }

        [Route("co/{coLimit}")]
        [HttpPost]
        public ActionResult Co(double coLimit)
        {
            _monitoringService.limits["co"] = coLimit;
            return Ok();
        }

        [Route("smoke/{smokeLimit}")]
        [HttpPost]
        public ActionResult Smoke(double smokeLimit)
        {
            _monitoringService.limits["smoke"] = smokeLimit;
            return Ok();
        }

        [Route("lpg/{lpgLimit}")]
        [HttpPost]
        public ActionResult Lpg(double lpgLimit)
        {
            _monitoringService.limits["lpg"] = lpgLimit;
            return Ok();
        }
    }
}