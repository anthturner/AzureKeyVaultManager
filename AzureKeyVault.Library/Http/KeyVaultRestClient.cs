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
    class KeyVaultRestClient
    {
        private readonly Uri _root;
        private readonly HttpClient _client;
        private readonly string _version;

        public KeyVaultRestClient(Guid subscriptionId, string resourceGroupName, HttpClient client)
        {
            _root = new Uri($"https://management.azure.com/subscriptions/{subscriptionId.ToString("D")}/resourceGroups/{resourceGroupName}/providers/Microsoft.KeyVault/");
            _client = client;
            _version = "2015-06-01";
        }

        public async Task<IEnumerable<AzureKeyVault>> GetVaults()
        {
            var uri = new Uri(_root, $"vaults?api-version={_version}");
            var data = await Get<JsonValues<AzureKeyVault>>(uri);
            return data.Value;
        }

        public async Task<AzureKeyVault> GetVault(string vaultName)
        {
            var uri = new Uri(_root, $"vaults/{vaultName}?api-version={_version}");
            return await Get<AzureKeyVault>(uri);
        }

        private async Task<T> Get<T>(Uri uri)
        {
            var result = await _client.GetAsync(uri);
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
