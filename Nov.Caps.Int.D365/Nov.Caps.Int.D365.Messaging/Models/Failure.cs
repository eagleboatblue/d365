using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Failure
    {
        [JsonProperty("flow", Required = Required.Always)]
        public string Flow { get; private set; }

        [JsonProperty("error", Required = Required.Always)]
        public Error Error { get; private set; }

        [JsonProperty("initial", Required = Required.Always)]
        public Payload<dynamic> Initial { get; private set; }

        [JsonProperty("current", Required = Required.AllowNull)]
        public Payload<dynamic> Current { get; private set; }

        public Failure(Error error, Payload<dynamic> initial) : this(error, initial, null) { }

        public Failure(Error error, Payload<dynamic> initial, Payload<dynamic> current)
        {
            this.Flow = "d365";
            this.Error = error;
            this.Initial = initial;
            this.Current = current;
        }
    }
}
