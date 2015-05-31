using System.Collections.Generic;
using System.Web.Http;

namespace Glimpse.WebApiV2.Sample.Controllers
{
    [RoutePrefix("api/Other")]
    public class OtherValuesController : ApiController
    {
        // GET api/other
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/other/5
        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/other
        public void Post([FromBody]string value)
        {
        }

        // PUT api/other/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/other/5
        public void Delete(int id)
        {
        }
    }
}