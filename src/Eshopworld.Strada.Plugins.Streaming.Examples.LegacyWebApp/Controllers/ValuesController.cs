using System.Web.Http;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public long Get()
        {
            return EventMetaCache.Instance.NumItems;
        }

        // GET api/values/5
        public void Get(int id)
        {
            var payload = new Message
            {
                Greeting = "Hey!",
                Value = id
            };

            EventMetaCache.Instance.Add(payload,
                "MAX",
                "CREATE",
                AspNet.Functions.GetFingerprint(Request)
            );
        }

        // POST api/values                      
        public void Post([FromBody] Message greeting)
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