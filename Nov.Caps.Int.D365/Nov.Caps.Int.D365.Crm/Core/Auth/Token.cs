using Newtonsoft.Json;
using System;

namespace Nov.Caps.Int.D365.Crm.Core.Auth
{
    public class Token
    {
        [JsonProperty("token_type", Required = Required.Always)]
        public string Type { get; private set; }

        [JsonProperty("scope", Required = Required.Always)]
        public string Scope { get; private set; }

        [JsonProperty("expires_on", Required = Required.Always)]
        public int ExpiresOn { get; private set; }

        [JsonProperty("resource", Required = Required.Always)]
        public string Resource { get; private set; }

        [JsonProperty("access_token", Required = Required.Always)]
        public string Access { get; private set; }

        [JsonProperty("refresh_token", Required = Required.Always)]
        public string Refresh { get; private set; }

        [JsonProperty("id_token", Required = Required.Always)]
        public string ID { get; private set; }

        public Token Clone()
        {
            var clone = new Token()
            {
                Access = this.Access,
                ExpiresOn = this.ExpiresOn,
                ID = this.ID,
                Refresh = this.Refresh,
                Resource = this.Resource,
                Scope = this.Scope,
                Type = this.Type
            };

            return clone;
        }

        public bool IsExpired()
        {
            var now = DateTimeOffset.Now.ToUniversalTime().ToUnixTimeMilliseconds();
            var expiration = Convert.ToInt64(this.ExpiresOn) * 1000;

            return now > expiration;
        }
    }
}
