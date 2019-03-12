using Microsoft.AspNetCore.Mvc;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorrelationController : ControllerBase
    {
        private readonly DataAnalyticsMeta _dataAnalyticsMeta;

        public CorrelationController(DataAnalyticsMeta dataAnalyticsMeta)
        {
            _dataAnalyticsMeta = dataAnalyticsMeta;
        }

        [HttpGet]
        public string Get()
        {
            return _dataAnalyticsMeta.Fingerprint;
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] Payload payload)
        {
            EventMetaCache.Instance.Add(payload, null, null, _dataAnalyticsMeta.Fingerprint);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}