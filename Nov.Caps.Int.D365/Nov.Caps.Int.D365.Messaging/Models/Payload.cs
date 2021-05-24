using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Payload<T>
    {
        [JsonProperty("metadata", Required = Required.Always)]
        public Metadata Metadata { get; private set; }

        [JsonProperty("data", Required = Required.Always)]
        public T Data { get; private set; }

        public Payload(Metadata metadata, T data)
        {
            this.Metadata = metadata;
            this.Data = data;
        }
    }
}
