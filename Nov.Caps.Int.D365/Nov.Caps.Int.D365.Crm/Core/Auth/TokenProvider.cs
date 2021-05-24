using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nov.Caps.Int.D365.Crm.Core.Auth
{
    public class TokenProvider
    {
        private Token value;
        private readonly SemaphoreSlim mutex;
        private readonly TokenProviderSettings settings;
        private readonly JsonSerializer serializer;
        private readonly HttpClient http;

        public Uri InstanceUri { get { return this.settings.InstanceUri; } }

        public TokenProvider(TokenProviderSettings settings, HttpClient http)
        {
            this.mutex = new SemaphoreSlim(1);
            this.settings = settings;
            this.serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,

                DefaultValueHandling = DefaultValueHandling.Populate,
            };
            this.http = http;
        }

        public async Task<Token> GetTokenAsync()
        {
            await this.mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                bool update = this.value != null ? this.value.IsExpired() : true;

                if (update)
                {
                    var assertion = await this.getAssertion();
                    var token = await this.getAuthToken(assertion);

                    this.value = token;
                }

                return this.value.Clone();
            }
            catch (Exception exception)
            {
                this.value = null;

                throw exception;
            }
            finally
            {
                this.mutex.Release();
            }
        }

        private async Task<Token> getAuthToken(string assertion)
        {
            if (String.IsNullOrWhiteSpace(assertion))
            {
                throw new Exception("Missed authentication assertion");
            }

            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("assertion", assertion),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:saml1_1-bearer"),
                new KeyValuePair<string, string>("scope", "openid"),
                new KeyValuePair<string, string>("username", this.settings.Username),
                new KeyValuePair<string, string>("password", this.settings.Password),
                new KeyValuePair<string, string>("client_id", this.settings.ClientID),
                new KeyValuePair<string, string>("client_secret", this.settings.ClientSecret),
                new KeyValuePair<string, string>("resource", this.settings.InstanceUri.ToString())
            };

            var url = this.settings.Authority;
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new FormUrlEncodedContent(body);
            req.Headers.Add("Host", "login.microsoftonline.com");

            using (var response = await this.http.SendAsync(req))
            {
                var str = await response.Content.ReadAsStringAsync();

                if (String.IsNullOrWhiteSpace(str))
                {
                    throw new HttpRequestException($"No content in response", new HttpRequestException($"{url}: {response.StatusCode}"));
                }

                if (response.IsSuccessStatusCode)
                {
                    return this.deserialize<Token>(str);
                }

                var err = this.deserialize<Error>(str);

                throw new HttpRequestException(err.Description);
            }
        }

        private async Task<string> getAssertion()
        {
            var sb = new StringBuilder();
            var now = DateTime.Now.ToUniversalTime();
            var createdTs = now.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            var expiresTs = now.AddMinutes(60).ToString("yyyy-MM-ddThh:mm:ss.fffZ");

            var soap = $@"
                <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" xmlns:a=""http://www.w3.org/2005/08/addressing"">
                    <s:Header>
                        <a:Action s:mustUnderstand=""1"">http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>
                        <a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo>
                        <a:To s:mustUnderstand=""1"">{this.settings.AdfsUri}</a:To>
                        <Security s:mustUnderstand=""1"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                            <UsernameToken u:Id=""{Guid.NewGuid()}"">
                                <Username>{this.settings.Username}</Username>
                                <Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{this.settings.Password}</Password>
                            </UsernameToken>
                        </Security>
                    </s:Header>
                    <s:Body>
                        <trust:RequestSecurityToken xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512"">
                            <wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
                                <a:EndpointReference>
                                    <a:Address>urn:federation:MicrosoftOnline</a:Address>
                                </a:EndpointReference>
                            </wsp:AppliesTo>
                            <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>
                            <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>
                        </trust:RequestSecurityToken>
                    </s:Body>
                </s:Envelope>
            ";

            var req = new HttpRequestMessage(HttpMethod.Post, this.settings.AdfsUri);
            req.Headers.Add("SOAPAction", "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/issue");
            req.Headers.Add("client-request-id", this.settings.ClientID);
            req.Headers.Add("return-client-request-id", "true");
            req.Headers.Add("Accept", "application/json");

            req.Content = new StringContent(soap, Encoding.UTF8, "application/soap+xml");

            using (var response = await this.http.SendAsync(req))
            {
                var dataStr = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Extracting assertion
                    var match = Regex.Match(dataStr, "<saml:Assertion[\\s\\S]*?<\\/saml:Assertion>", RegexOptions.ECMAScript);

                    if (!match.Success)
                    {
                        throw new Exception("Failed to get authentication assertion", new Exception(dataStr));
                    }

                    // Encoding to base64
                    var bytes = System.Text.Encoding.UTF8.GetBytes(match.ToString());
                    return Convert.ToBase64String(bytes);
                }

                var xml = XDocument.Parse(dataStr);
                var reasonNode = xml.Descendants()
                    .Where(c => c.Name.LocalName.ToString() == "Reason")
                    .Select(c => c.Descendants().Where(d => d.Name.LocalName.ToString() == "Text").Select(d => d.Value).First())
                    .First();
                var resonStr = reasonNode.ToString();

                throw new Exception("Failed to obtain SAML token", new HttpRequestException(resonStr));
            }
        }

        private T deserialize<T>(string input)
        {
            using (var sr = new StringReader(input))
            {
                return this.serializer.Deserialize<T>(new JsonTextReader(sr));
            }
        }
    }
}
