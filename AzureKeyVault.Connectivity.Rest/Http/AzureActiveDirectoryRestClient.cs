using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Rest.Serialization;

namespace AzureKeyVault.Connectivity.Rest.Http
{
    class AzureActiveDirectoryRestClient : RestClientBase, IAzureActiveDirectoryService
    {
        private readonly Uri _root;

        public AzureActiveDirectoryRestClient(HttpClient client, string tenant) : base(client, "1.6")
        {
            _root = new Uri($"https://graph.windows.net/{tenant}/");
        }

        public async Task<IEnumerable<IAzureActiveDirectoryUser>> SearchUsers(string searchString)
        {
            var uri = new Uri(_root, $"users/?api-version={Version}&top=100");
            var data = await Get<JsonValues<IAzureActiveDirectoryUser>>(uri);
            return data.Value;
        }
    }
}
