using AzureKeyVaultManager.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Http
{
    class RestClientBase
    {
        private readonly HttpClient _client;

        public RestClientBase(HttpClient client, string version)
        {
            _client = client;
            Version = version;
        }

        protected string Version { get; }

        protected async Task<T> Get<T>(Uri uri)
        {
            var result = await _client.GetAsync(uri);
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
