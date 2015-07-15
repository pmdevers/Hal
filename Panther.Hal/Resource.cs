using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Reflection;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Panther.Hal
{
    public abstract class Resource : IResource
    {
        [JsonIgnore]
        public virtual string Href { get; set; }
        [JsonIgnore]
        public virtual string Rel { get; set; }

        protected Resource()
        {
            Links = new List<Link>();
            Actions = new List<Action>();
        }

        [JsonProperty("_links")]
        public IList<Link> Links { get; set; }

        [JsonProperty("_actions")]
        public IList<Action> Actions { get; set; }

        [JsonProperty("_embedded")]
        private IList<EmbeddedResource> Resources { get; set; }

        internal static bool IsEmbeddedResourceType(Type type)
        {
            return typeof(IResource).IsAssignableFrom(type) ||
                   typeof(IEnumerable<IResource>).IsAssignableFrom(type);
        }

        internal void ResetResources()
        {
            foreach (var property in properties.Keys)
            {
                property.SetValue(this, properties[property]);
            }
        }

        internal void PopulateHyperMedia()
        {
            PopulateEmbeddedResources();
            CreateHypermedia();

            if (!string.IsNullOrEmpty(Href) && Links.Count(l => l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });
        }

        private Dictionary<PropertyInfo, object> properties = new Dictionary<PropertyInfo, object>();

        private void PopulateEmbeddedResources()
        {
            var resources = GetType().GetProperties().Where(x => IsEmbeddedResourceType(x.PropertyType));

            Resources = new List<EmbeddedResource>();

            foreach (var propertyInfo in resources)
            {
                var value = propertyInfo.GetValue(this);

                if(value == null)
                    continue;

                properties.Add(propertyInfo, value);

                var resource = value as IResource;

                if (resource != null)
                    CreateEmbeddedResource(resource);
                else
                    CreateEmbeddedResource((IEnumerable<IResource>)value);
                
                propertyInfo.SetValue(this, null);
            }

            if (Resources.Count == 0)
                Resources = null;
        }



        private void CreateEmbeddedResource(IEnumerable<IResource> value)
        {
            Resources.Add(new EmbeddedResource { IsSourceAnArray = true, Resources = value.ToList()});
        }

        private void CreateEmbeddedResource(IResource resource)
        {
            Resources.Add(new EmbeddedResource { IsSourceAnArray = true, Resources = new[] { resource } });
        }

        protected internal virtual void CreateHypermedia()
        {
            
        }
    }
}
