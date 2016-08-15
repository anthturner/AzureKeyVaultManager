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
            var uri = new Uri(_root, $"users/?api-version={Version}&$filter=accountEnabled eq true and (startswith(userPrincipalName,'{searchString}') or startswith(displayName,'{searchString}'))");
            var data = await Get<JsonValues<AzureActiveDirectoryUser>>(uri);
            return data.Value;
        }

        public async Task<IEnumerable<IAzureActiveDirectoryUser>> GetUsers(string[] userIds)
        {
            var list = new List<AzureActiveDirectoryUser>();
            foreach (var userId in userIds)
            {
                var uri = new Uri(_root, $"users/{userId}?api-version={Version}");
                var data = await Get<AzureActiveDirectoryUser>(uri);
                list.Add(data);
            }
            return list;
        }

        public async Task<string> MyObjectId()
        {
            var uri = new Uri(new Uri("https://graph.windows.net/"), $"me?api-version={Version}");
            var data = await Get<dynamic>(uri);
            return (string)data.objectId;
        }
    }
}
