using System;

namespace Nov.Caps.Int.D365.Crm.Core
{
    public class ApiClientException : Exception
    {
        public string Code { get; private set; }

        public ApiClientException(string code, string message) : base($"Api Error: {message}")
        {
            this.Code = code;
        }

        public ApiClientException(ErrorMessage message) : this(message.Code, message.Message) { }
    }
}
