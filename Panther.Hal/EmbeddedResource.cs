using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panther.Hal
{
    public class EmbeddedResource 
    {
        public EmbeddedResource()
        {
            Resources = new List<IResource>();
        }

        public bool IsSourceAnArray { get; set; }

        public IList<IResource> Resources { get; set; }

        internal void PopulateResources()
        {
            foreach (var resource in Resources)
            {
                var res = resource as Resource;
                res?.PopulateHyperMedia();
            }
        }
    }
}
