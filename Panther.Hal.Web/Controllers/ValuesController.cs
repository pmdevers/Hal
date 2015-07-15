using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using Panther.Hal.Web.Model;
using Panther.Hal.Web.Representations;

namespace Panther.Hal.Web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public TestResource Get()
        {
            var test =  new TestResource() { TestProperty = "testProperty", Rel = "test1", Href = "/api/test1"};
            var test1 = new TestResource() { TestProperty = "testProperty111", Rel = "test2", Href = "/api/test12" };

            test.Actions.Add(Action.CreateAction<PostModel>("CreateNew", "/api/test1"));

            //test.Resources.Add(new TestEmbedded() { Title = "testtrest", Rel = "test", Href = "fdfdfdf"});
            //test.Resources = new List<EmbeddedResource>();
            //test.Resources.Add(new EmbeddedResource { IsSourceAnArray = true, Resources = new[] { test1, test1 } });
            return test;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
