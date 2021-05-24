using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Crm.Core
{
    public class ApiError
    {
        [JsonProperty("error", Required = Required.Always)]
        public ErrorMessage Error { get; private set; }
    }
}
