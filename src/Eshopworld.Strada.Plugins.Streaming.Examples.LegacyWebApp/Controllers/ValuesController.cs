using System.Web.Http;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public long Get()
        {
            return EventMetadataCache.Instance.NumItems;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return null;
        }

        // POST api/values                      
        public void Post([FromBody] string value)
        {
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