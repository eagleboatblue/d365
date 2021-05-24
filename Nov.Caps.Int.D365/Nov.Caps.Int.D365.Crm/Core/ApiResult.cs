using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Crm.Core
{
    public class ApiResult<T>
    {
        [JsonProperty("value", Required = Required.Always)]
        public T Value { get; private set; }
    }
}
