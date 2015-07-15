using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Newtonsoft.Json;

namespace Panther.Hal
{
    public class Action
    {
        private static readonly Regex isTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

        internal Action(Type type, string rel, string href, string verb)
        {
            model = type.GetProperties().ToDictionary(key => key.Name, value => value.PropertyType.Name);
            Rel = rel;
            Href = href;
            Verb = verb;
        }

        [JsonProperty("verb")]
        public string Verb { get; private set; }

        [JsonIgnore]
        public string Rel { get; private set; }

        [JsonProperty("href")]
        public string Href { get; private set; }

        public bool IsTemplated
        {
            get { return isTemplatedRegex.IsMatch(Href); }
        }

        [JsonProperty("model")]
        private Dictionary<string, string> model;


        public static Action CreateAction<T>(string rel, string href, string verb = "POST")
        {
            return new Action(typeof(T), rel, href, verb);
        }
    }
}
