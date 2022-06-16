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
        private IMemoryCache _cache;
        private double temp;
        private double humidity;
        private double smoke;
        private double co;
        private double lpg;

        public MonitoringController(ILogger<MonitoringController> logger, MonitoringService ms, IMemoryCache cache)
        {
            _logger = logger;
            _monitoringService = ms;
            _cache = cache;
            InitVariables();

        }

        private void InitVariables()
        {
            double cacheValueTemp, cacheValueHumidity, cacheValueCO, cacheValueSmoke, cacheValueLpg;
            if(!_cache.TryGetValue("temp", out cacheValueTemp))
            {
                cacheValueTemp = 0.0;
                temp = 0.0;
                _cache.Set("temp", cacheValueTemp);
            }
            else
            {
                temp = cacheValueTemp;
            }

            if (!_cache.TryGetValue("humidity", out cacheValueHumidity))
            {
                cacheValueHumidity = 50.0;
                humidity = 50.0;
                _cache.Set("humidity", cacheValueHumidity);
            }
            else
            {
                humidity = cacheValueHumidity;
            }

            if (!_cache.TryGetValue("co", out cacheValueCO))
            {
                cacheValueCO = 0.0;
                co = 0.0;
                _cache.Set("co", cacheValueCO);
            }
            else
            {
                co = cacheValueCO;
            }

            if (!_cache.TryGetValue("lpg", out cacheValueLpg))
            {
                cacheValueLpg = 0.0;
                lpg = 0.0;
                _cache.Set("lpg", cacheValueLpg);
            }
            else
            {
                lpg = cacheValueLpg;
            }

            if (!_cache.TryGetValue("smoke", out cacheValueSmoke))
            {
                cacheValueSmoke = 0.0;
                smoke = 0.0;
                _cache.Set("smoke", cacheValueSmoke);
            }
            else
            {
                smoke = cacheValueSmoke;
            }
        }

        [Route("temp/{tempLimit}")]
        [HttpPost]
        public ActionResult Temperature(double tempLimit)
        {
            _cache.Set("temp", tempLimit);
            return Ok();
        }

        [Route("humidity/{humidityLimit}")]
        [HttpPost]
        public ActionResult Humidity(double humidityLimit)
        {
            _cache.Set("humidity", humidityLimit);
            return Ok();
        }

        [Route("co/{coLimit}")]
        [HttpPost]
        public ActionResult Co(double coLimit)
        {
            _cache.Set("co", coLimit);
            return Ok();
        }

        [Route("smoke/{smokeLimit}")]
        [HttpPost]
        public ActionResult Smoke(double smokeLimit)
        {
            _cache.Set("smoke", smokeLimit);
            return Ok();
        }

        [Route("lpg/{lpgLimit}")]
        [HttpPost]
        public ActionResult Lpg(double lpgLimit)
        {
            _cache.Set("lpg", lpgLimit);
            return Ok();
        }
    }
}