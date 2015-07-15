using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panther.Hal.Web.Representations
{
    public class TestResource : Resource
    {
        public TestResource()
        {
            Test2 = new List<TestResource2>();

            Test2.Add(new TestResource2 {Href = "/api/test", Rel = "Resource1"});
            Test2.Add(new TestResource2 { Href = "/api/test", Rel = "Resource2" });

            Test3 = new Dictionary<string, string>();

            Test4 = new List<TestResource2>();

            Test4.Add(new TestResource2 { Href = "/api/test", Rel = "Resource3" });
            Test4.Add(new TestResource2 { Href = "/api/test", Rel = "Resource3" });

            Test3.Add("test", "test3");
        }
        public override string Rel { get { return "TestResource"; } set {} }
        public override string Href { get { return "/api/value/{test}"; } set {} }

        public string TestProperty { get; set; }

        public List<TestResource2> Test2 { get; set; }
        public List<TestResource2> Test4 { get; set; }

        public Dictionary<string, string> Test3 { get; set; }
    }

    public class TestResource2 : Resource
    {
        
    }
}
