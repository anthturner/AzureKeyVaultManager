using AzureKeyVaultManager.Contracts;
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
    }
}
