using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

using Newtonsoft.Json;

namespace Panther.Hal.Converters
{
    public class ActionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var actions = (IList<Action>)value;

            if (actions.Count == 0)
                return;

            writer.WriteStartObject();

            foreach (var action in actions)
            {
                writer.WritePropertyName(action.Rel);
                serializer.Serialize(writer, action);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<Action>).IsAssignableFrom(objectType);
        }
    }
}
