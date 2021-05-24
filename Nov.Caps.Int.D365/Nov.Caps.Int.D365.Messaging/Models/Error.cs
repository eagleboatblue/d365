using Newtonsoft.Json;
using System;

namespace Nov.Caps.Int.D365.Messaging.Models
{
    public class Error
    {
        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; private set; }

        [JsonProperty("source", Required = Required.AllowNull)]
        public Error Source { get; private set; }

        public Error(string message) : this(message, null) { }

        public Error(string message, Exception source)
        {
            this.Message = message;
            this.Source = source != null ? new Error(source.Message, source.InnerException) : null;
        }
    }
}
