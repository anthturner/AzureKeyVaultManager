using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultService
    {
        public List<Tuple<string, string>> Vaults { get; private set; }

        private KeyVaultManagementClient VaultManagementClient { get; }
        private KeyVaultClient VaultClient { get; }

        public KeyVaultService(SubscriptionCloudCredentials credentials)
        {
            VaultManagementClient = new KeyVaultManagementClient(credentials);
            VaultClient = new KeyVaultClient(GetKeyVaultAccessToken);
        }
        
        public async Task<KeyVault> GetKeyVault(string resourceGroup, string name)
        {
            var vault = (await VaultManagementClient.Vaults.GetAsync(resourceGroup, name)).Vault;
            return new KeyVault(VaultClient, vault);
        }

        public async Task RefreshVaults()
        {
            Vaults = new List<Tuple<string, string>>();

            var resourceGroups = await AdalHelper.Instance.GetResourceGroups();
            foreach (var group in resourceGroups)
            {
                var response = await VaultManagementClient.Vaults.ListAsync(group, 50, CancellationToken.None);
                Vaults.AddRange(response.Vaults.Select(v => Tuple.Create(group, v.Name)));
                string nextLink = response.NextLink;
                while (!string.IsNullOrEmpty(response.NextLink))
                {
                    var nextResponse = await VaultManagementClient.Vaults.ListNextAsync(nextLink, CancellationToken.None);
                    Vaults.AddRange(nextResponse.Vaults.Select(v => Tuple.Create(group, v.Name)));
                    nextLink = response.NextLink;
                }
            }
        }

        private async Task<string> GetKeyVaultAccessToken(string authority, string resource, string scope)
        {
            var clientCredential = new ClientCredential(AdalHelper.KeyVaultClientId, AdalHelper.KeyVaultClientSecret);
            var context = new AuthenticationContext(authority, null);
            var result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }
    }
}