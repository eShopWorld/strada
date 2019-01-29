using System.Web.Http;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly EventMetadataCache _eventMetadataCache;

        public ValuesController(EventMetadataCache eventMetadataCache)
        {
            _eventMetadataCache = eventMetadataCache;
        }

        // GET api/values
        public bool Get()
        {
            return true;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
            _eventMetadataCache.Add(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}