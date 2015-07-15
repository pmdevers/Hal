using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panther.Hal
{
    public interface ICollectionResource<TResource> : IResource
        where TResource : IResource
    {
        ICollection<TResource> Items { get; set; }
    }
}
