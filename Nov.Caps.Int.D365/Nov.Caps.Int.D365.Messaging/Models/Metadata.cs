using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Metadata
    {
        [JsonProperty("jobID", Required = Required.Always)]
        public string JobID { get; private set; }

        [JsonProperty("user", Required = Required.Always)]
        public string User { get; private set; }

        [JsonProperty("attributes", Required = Required.Default)]
        public ReadOnlyDictionary<string, object> Attributes { get; private set; }

        [JsonProperty("target", Required = Required.Always)]
        public Target Target { get; private set; }
    }
}
