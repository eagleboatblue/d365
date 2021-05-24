using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core.Auth;
using System;
using System.Net;
using Newtonsoft.Json.Serialization;
using Nov.Caps.Int.D365.Crm.Core.OData;

namespace Nov.Caps.Int.D365.Crm.Core
{
    public class ApiClient
    {
        private readonly TokenProvider tokenProvider;
        private readonly JsonSerializer dserializer;
        private readonly HttpClient http;

        public ApiClient(TokenProvider tokenProvider, HttpClient http)
        {
            this.tokenProvider = tokenProvider;
            this.dserializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,

                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            this.http = http;
        }

        public Task<T> GetAsync<T>(string path) where T : class
        {
            return this.SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, this.toUrl(path)));
        }

        public Task<TOut> PostAsync<TIn, TOut>(string path, TIn payload) where TOut : class
        {
            return this.SendDataAsync<TIn, TOut>(
                new HttpRequestMessage(HttpMethod.Post, this.toUrl(path)),
                payload
            );
        }

        public Task<TOut> PutAsync<TIn, TOut>(string path, TIn payload) where TOut : class
        {
            return this.SendDataAsync<TIn, TOut>(
                new HttpRequestMessage(HttpMethod.Put, this.toUrl(path)),
                payload
            );
        }

        public Task<TOut> PatchAsync<TIn, TOut>(string path, TIn payload) where TOut : class
        {
            return this.SendDataAsync<TIn, TOut>(
                new HttpRequestMessage(new HttpMethod("PATCH"), this.toUrl(path)),
                payload
            );
        }

        private Task<TOut> SendDataAsync<TIn, TOut>(HttpRequestMessage req, TIn data) where TOut : class
        {
            using (var writer = new StringWriter())
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,

                    DefaultValueHandling = DefaultValueHandling.Populate,

                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new ReferenceNamingStrategy(typeof(TIn))
                        {
                            OverrideSpecifiedNames = false,
                            ProcessDictionaryKeys = true,
                            ProcessExtensionDataNames = true
                        }
                    }
                };

                serializer.Serialize(writer, data);

                req.Content = new StringContent(writer.ToString(), Encoding.UTF8, "application/json");

                return this.SendAsync<TOut>(req);
            }
        }

        private async Task<T> SendAsync<T>(HttpRequestMessage request) where T : class
        {
            var authToken = await this.tokenProvider.GetTokenAsync();

            request.Headers.Add("Authorization", $"Bearer {authToken.Access}");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("OData-MaxVersion", "4.0");
            request.Headers.Add("OData-Version", "4.0");

            using (var response = await this.http.SendAsync(request))
            {
                var str = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (String.IsNullOrWhiteSpace(str))
                    {
                        return null;
                    }

                    return this.deserializeResult<T>(str);
                }

                var err = this.deserializeError(str, response.StatusCode);

                throw new ApiClientException(err);
            }
        }

        private string toUrl(string path)
        {
            return $"{this.tokenProvider.InstanceUri}/{path}";
        }

        private T deserializeResult<T>(string input)
        {
            using (var sr = new StringReader(input))
            {
                var result = this.dserializer.Deserialize<ApiResult<T>>(new JsonTextReader(sr));

                return result.Value;
            }
        }

        private ErrorMessage deserializeError(string input, HttpStatusCode statusCode)
        {
            using (var sr = new StringReader(input))
            {
                try
                {
                    var result = this.dserializer.Deserialize<ApiError>(new JsonTextReader(sr));

                    return result.Error;
                }
                catch
                {
                    return new ErrorMessage(statusCode.ToString(), input);
                }
            }
        }
    }
}
