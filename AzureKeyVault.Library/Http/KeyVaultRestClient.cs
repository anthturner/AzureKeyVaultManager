using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureKeyVaultManager.Http
{
    class KeyVaultRestClient : RestClientBase, IKeyVaultService
    {
        private readonly Uri _root;

        public KeyVaultRestClient(IKeyVault keyVault, HttpClient client)
            : base(client, "2015-06-01")
        {
            _root = keyVault.Uri;
        }

        public async Task<ICollection<IKeyVaultSecret>> GetSecrets()
        {
            var uri = new Uri(_root, $"secrets?api-version={Version}");
            var data = await Get<JsonValues<AzureKeyVaultSecret>>(uri);
            return data.Value.Select(x => x as IKeyVaultSecret).ToList();
        }

        public async Task<String> GetSecretValue(IKeyVaultSecret secret)
        {
            var uri = new Uri(_root, $"secrets/{secret.Name}?api-version={Version}");
            var data = await Get<AzureKeyVaultSecretValue>(uri);
            return data.Value;
        }

        public async Task<ICollection<IKeyVaultKey>> GetKeys()
        {
            var uri = new Uri(_root, $"keys?api-version={Version}");
            var data = await Get<JsonValues<AzureKeyVaultKey>>(uri);
            return data.Value.Select(x => x as IKeyVaultKey).ToList();
        }

        public async Task<String> GetKeyValue(IKeyVaultKey key)
        {
            var uri = new Uri(_root, $"keys/{key.Name}?api-version={Version}");
            var data = await Get<AzureKeyVaultKeyValue>(uri);
            return JsonConvert.SerializeObject(new { key = data.Key });
        }
    }
}
