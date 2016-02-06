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
    class AzureRestClient : RestClientBase, IAzureManagementService
    {
        private readonly Uri _root;

        public AzureRestClient(HttpClient client)
            : base(client, "2015-01-01")
        {
            _root = new Uri($"https://management.azure.com/subscriptions/");
        }

        public async Task<IEnumerable<ISubscription>> GetSubscriptions()
        {
            var uri = new Uri(_root, $"?api-version={Version}");
            var data = await Get<JsonValues<AzureSubscription>>(uri);
            return data.Value;
        }
    }
}
