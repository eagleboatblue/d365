using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Target
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; private set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; private set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }

        [JsonProperty("operation", Required = Required.Always)]
        public string Operation { get; private set; }
    }
}
