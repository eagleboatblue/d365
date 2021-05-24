using Newtonsoft.Json;

namespace Nov.Caps.Int.D365.Crm.Core
{
    public class ErrorMessage
    {
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; private set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; private set; }

        public ErrorMessage(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
