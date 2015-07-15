using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

using Microsoft.AspNet.Http;

namespace Panther.Hal.Converters
{
    public class LinksConverter :JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = (IList<Link>)value;

            writer.WriteStartObject();

            var groupedByRel = links
                .GroupBy(link => link.Rel)
                .ToDictionary(link => link.Key, link => link.ToArray());

            var allRelsExceptWellknown = groupedByRel.Keys
                .Where(key => key != Link.CuriesRel && key != Link.SelfRel).ToList();

            if (groupedByRel.ContainsKey(Link.SelfRel))
            {
                WriteLinks(writer, serializer, Link.SelfRel, groupedByRel[Link.SelfRel]);
            }

            if (groupedByRel.ContainsKey(Link.CuriesRel))
            {
                WriteLinks(writer, serializer, Link.CuriesRel, groupedByRel[Link.CuriesRel]);
            }

            foreach (var rel in allRelsExceptWellknown)
            {
                WriteLinks(writer, serializer, rel, groupedByRel[rel]);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var links = new List<Link>();

            var jToken = JToken.ReadFrom(reader);
            foreach (var jProperty in jToken.Where(prop => prop.GetType() == typeof(JProperty)).Cast<JProperty>())
            {
                if (jProperty.Value.GetType() == typeof(JArray))
                {
                    var linksObj = serializer.Deserialize<Link[]>(jProperty.Value.CreateReader());
                    foreach (var link in linksObj)
                    {
                        links.Add(link);
                        link.Rel = jProperty.Name;
                    }
                }
                else
                {
                    var link = serializer.Deserialize<Link>(jProperty.Value.CreateReader());
                    link.Rel = jProperty.Name;
                    links.Add(link);
                }
            }

            return links;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<Link>).IsAssignableFrom(objectType);
        }

        void WriteLinks(JsonWriter writer, JsonSerializer serializer, string rel, Link[] links)
        {
            writer.WritePropertyName(rel);

            if (links.Any() || rel == Link.CuriesRel)
            {
                foreach (var link in links)
                {

                    WriteLink(writer, link);
                    //serializer.Serialize(writer, link);
                }
            }
            else if (links.Length == 1)
            {
                serializer.Serialize(writer, links[0]);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected: Found an empty arry for link group: {rel}");
            }
        }

        private void WriteLink(JsonWriter writer, Link link)
        {

            writer.WriteStartObject();

            foreach (var info in link.GetType().GetProperties())
            {
                switch (info.Name.ToLowerInvariant())
                {
                    case "href":
                        writer.WritePropertyName("href");
                        writer.WriteValue(ResolveUri(link.Href));
                        break;
                    case "rel":
                        // do nothing ...
                        break;
                    case "istemplated":
                        if (link.IsTemplated)
                        {
                            writer.WritePropertyName("templated");
                            writer.WriteValue(true);
                        }
                        break;
                    case "curieprefix":
                        //do nothing
                        break;
                    default:
                        if ((info.PropertyType == typeof(string)))
                        {
                            var text = info.GetValue(link) as string;

                            if (string.IsNullOrEmpty(text))
                                continue; // no value set, so don't write this property ...

                            writer.WritePropertyName(info.Name.ToLowerInvariant());
                            writer.WriteValue(text);
                        }
                        // else: no sensible way to serialize ...
                        break;
                }
            }

            writer.WriteEndObject();

        }

        public virtual string ResolveUri(string href)
        {
            if (!string.IsNullOrEmpty(href))
                return href.StartsWith("~/") ? href.Replace("~/", "/") : href;
            return href;
        }
    }
}
