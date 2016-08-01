using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureKeyVault.Connectivity.KeyVaultWrapper;
using Newtonsoft.Json;

namespace AzureKeyVault.Connectivity.Rest.Http
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
            if (data.Value == null)
                return new List<IKeyVaultSecret>();
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
            if (data.Value == null)
                return new List<IKeyVaultKey>();
            return data.Value.Select(x => x as IKeyVaultKey).ToList();
        }

        public async Task<String> GetKeyValue(IKeyVaultKey key)
        {
            var uri = new Uri(_root, $"keys/{key.Name}?api-version={Version}");
            var data = await Get<AzureKeyVaultKeyValue>(uri);
            if (data == null)
                return null;
            return JsonConvert.SerializeObject(new { key = data.Key });
        }

        public async Task Delete(IKeyVaultKey key)
        {
            var uri = new Uri(_root, $"keys/{key.Name}?api-version={Version}");
            await Delete(uri);
        }

        public async Task Delete(IKeyVaultSecret secret)
        {
            var uri = new Uri(_root, $"secrets/{secret.Name}?api-version={Version}");
            await Delete(uri);
        }

        public async Task<string> Encrypt(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueData)
        {
            var uri = new Uri(_root, $"keys/{key.Name}/encrypt?api-version={Version}");
            var command = new
            {
                alg = algorithm.GetConfigurationString(),
                value = valueData
            };
            var data = await Post(uri, JsonConvert.SerializeObject(command));
            return data.value;
        }

        public async Task<string> Decrypt(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueData)
        {
            var uri = new Uri(_root, $"keys/{key.Name}/decrypt?api-version={Version}");
            var command = new
            {
                alg = algorithm.GetConfigurationString(),
                value = valueData
            };
            var data = await Post(uri, JsonConvert.SerializeObject(command));
            return data.value;
        }

        public async Task<string> Sign(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string digest)
        {
            var uri = new Uri(_root, $"keys/{key.Name}/sign?api-version={Version}");
            var command = new
            {
                alg = algorithm.GetConfigurationString(),
                value = digest
            };
            var data = await Post(uri, JsonConvert.SerializeObject(command));
            return data.value;
        }

        public async Task<bool> Verify(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string digest, string valueToVerify)
        {
            var uri = new Uri(_root, $"keys/{key.Name}/verify?api-version={Version}");
            var command = new
            {
                alg = algorithm.GetConfigurationString(),
                digest = digest,
                value = valueToVerify
            };
            var data = await Post(uri, JsonConvert.SerializeObject(command));
            return data.value;
        }
    }
}
