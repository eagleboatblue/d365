using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Crm.Core.Auth
{
    public class Error
    {
        [JsonProperty("error", Required = Required.Always)]
        public string Message { get; private set; }

        [JsonProperty("error_description", Required = Required.Always)]
        public string Description { get; private set; }
    }
}
