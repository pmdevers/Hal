using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Serialization;

namespace Panther.Hal
{
    public interface IResource
    {
        IList<Link> Links { get; set; }
        //IList<IResource> Resources { get; set; }
        string Href { get; set; }
        string Rel { get; set; }
    }
}
