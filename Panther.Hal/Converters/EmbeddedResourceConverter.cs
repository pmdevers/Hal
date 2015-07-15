using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

using Newtonsoft.Json;

namespace Panther.Hal.Converters
{
    public class EmbeddedResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resourceList = (IList<EmbeddedResource>)value;
            if(resourceList.Count == 0) return;

            writer.WriteStartObject();

            foreach (var resource in resourceList)
            {
                resource.PopulateResources();
                writer.WritePropertyName(NormalizeRel(resource.Resources[0]));

                if(resource.IsSourceAnArray) writer.WriteStartArray();

                foreach (var res in resource.Resources)
                    serializer.Serialize(writer, res);

                if (resource.IsSourceAnArray) writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        private static string NormalizeRel(IResource resource)
        {
            //return resource.GetType().Name;
            if (!string.IsNullOrEmpty(resource.Rel))
                return resource.Rel;
            return "unknownRel-" + resource.GetType().Name;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<EmbeddedResource>).IsAssignableFrom(objectType);
        }
    }
}
