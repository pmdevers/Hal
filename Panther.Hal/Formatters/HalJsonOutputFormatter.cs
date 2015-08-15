using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using Panther.Hal.Converters;

namespace Panther.Hal.Formatters
{
    public class HalJsonOutputFormatter : OutputFormatter
    {
        public const string HalMediaType = "application/hal+json";

        readonly JsonSerializerSettings serializerSettings;

        public HalJsonOutputFormatter()
        {
            this.SupportedEncodings.Add(Encoding.UTF8);
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(HalMediaType));

            serializerSettings = new JsonSerializerSettings();

            
            serializerSettings.Converters.Add(new LinksConverter());
            serializerSettings.Converters.Add(new EmbeddedResourceConverter());
            serializerSettings.Converters.Add(new ActionConverter());
            serializerSettings.Converters.Add(new ResourceConverter());
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterContext context)
        {
            var response = context.HttpContext.Response;
            var selectedEncoding = context.SelectedEncoding;

            using (var writer = new HttpResponseStreamWriter(response.Body, selectedEncoding))
            {
                WriteObject(writer, context.Object);
            }

            return Task.FromResult(true);
        }

        private void WriteObject(TextWriter writer, object value)
        {
            using (var jsonWriter = CreateJsonWriter(writer))
            {
                var jsonSerializer = CreateJsonSerializer();
                jsonSerializer.Serialize(jsonWriter, value);
            }
        }

        private JsonSerializer CreateJsonSerializer()
        {
            return JsonSerializer.Create(serializerSettings);
        }

        private JsonWriter CreateJsonWriter(TextWriter writer)
        {
            return new JsonTextWriter(writer) { CloseOutput = false };
        }
    }
}
