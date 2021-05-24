using System;
namespace Nov.Caps.Int.D365.Crm.Core.Auth
{
    public class TokenProviderSettings
    {
        public Uri InstanceUri { get; set; }

        public Uri AdfsUri { get; set; }

        public string Authority { get; set; }

        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
