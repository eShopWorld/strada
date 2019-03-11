using System.Threading.Tasks;
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
        public async Task<string> Get(int id)
        {
            var payload = new Message {Greeting = "Hey!"};

            var httpRequestMeta =
                await HttpRequestMeta.Create(
                    Request,
                    AspNet.Functions.GetFingerprint(Request),
                    payload);

            EventMetaCache.Instance.Add(httpRequestMeta); // todo: call above as individual params?
            return httpRequestMeta.Fingerprint;
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