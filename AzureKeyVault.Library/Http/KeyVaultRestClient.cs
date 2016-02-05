using System;
using System.Collections.Generic;
using System.Linq;
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

        public KeyVaultRestClient(Guid subscriptionId, string resourceGroupName)
        {
            _root = new Uri($"https://management.azure.com/subscriptions/{subscriptionId.ToString("D")}/resourceGroups/{resourceGroupName}/providers/Microsoft.KeyVault");
            _client = new HttpClient();
            _version = "2015-06-01";
        }

        public async Task<string> GetVaults(string vaultName)
        {
            var result = await _client.GetAsync(new Uri(_root, $"vaults/{vaultName}?api-version={_version}"));
            return await result.Content.ReadAsStringAsync();
        }
    }
}
