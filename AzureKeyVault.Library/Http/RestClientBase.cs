using Newtonsoft.Json;
using System;
using System.Net.Http;
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
            return await GetResult<T>(result);
        }

        protected async Task Delete(Uri uri)
        {
            var result = await _client.DeleteAsync(uri);
            await GetResult(result);
        }

        private async Task GetResult(HttpResponseMessage response)
        {
            await GetResult<object>(response);
        }

        private async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                var errorResult = (dynamic)JsonConvert.DeserializeObject(content);
                if (errorResult != null && errorResult.error.message != null)
                    throw new Exception(errorResult.error.message, ex);
                throw;
            }

            if (!string.IsNullOrEmpty(content))
                return JsonConvert.DeserializeObject<T>(content);
            else
                return default(T);
        }
    }
}
