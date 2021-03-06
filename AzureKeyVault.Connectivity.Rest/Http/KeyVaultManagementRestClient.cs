﻿using AzureKeyVault.Connectivity.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Rest.Http
{
    class KeyVaultManagementRestClient : RestClientBase
    {
        private readonly Uri _root;

        public KeyVaultManagementRestClient(Guid subscriptionId, string resourceGroupName, HttpClient client)
            : base(client, "2015-06-01")
        {
            _root = new Uri($"https://management.azure.com/subscriptions/{subscriptionId.ToString("D")}/resourceGroups/{resourceGroupName}/providers/Microsoft.KeyVault/");
        }

        public async Task<IEnumerable<Serialization.AzureKeyVault>> GetVaults()
        {
            var uri = new Uri(_root, $"vaults?api-version={Version}");
            var data = await Get<JsonValues<Serialization.AzureKeyVault>>(uri);
            return data.Value;
        }

        public async Task<Serialization.AzureKeyVault> GetVault(string vaultName)
        {
            var uri = new Uri(_root, $"vaults/{vaultName}?api-version={Version}");
            return await Get<Serialization.AzureKeyVault>(uri);
        }

        public async Task DeleteVault(string vaultName)
        {
            var uri = new Uri(_root, $"vaults/{vaultName}?api-version={Version}");
            await Delete(uri);
        }
    }
}
