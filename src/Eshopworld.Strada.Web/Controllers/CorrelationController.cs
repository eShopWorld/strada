using Microsoft.AspNetCore.Mvc;

namespace Eshopworld.Strada.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorrelationController : ControllerBase
    {
        private readonly DataAnalytics _dataAnalytics;


        public CorrelationController(DataAnalytics dataAnalytics)
        {
            _dataAnalytics = dataAnalytics;
        }

        [HttpGet]
        public string Get()
        {
            var correlationId = _dataAnalytics.GetCorrelationId(Request);
            return correlationId ?? "???";
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
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