using System.Collections.Generic;
using System.Dynamic;
using System.Web.Http;
using Eshopworld.Strada.Plugins.Streaming.NetFramework;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.AspNet.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5
        public void Get(int id)
        {
            dynamic d = new ExpandoObject();
            d.Name = "TEST";
            d.Age = 12;

            EventMetaCache.Instance.Add(d, "D", "D",
                Functions.GetFingerprint(Request), "D",
                Functions.ParseHttpHeaders(Request.Headers));
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