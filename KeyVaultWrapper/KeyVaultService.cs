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
        public List<KeyVault> Vaults { get; private set; }

        private KeyVaultManagementClient VaultManagementClient { get; }
        private KeyVaultClient VaultClient { get; }

        public static CancellationToken GetTimeoutToken(TimeSpan timeout = new TimeSpan())
        {
            if (timeout.Ticks == 0)
                timeout = new TimeSpan(0,0,10); // default is 10s

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);
            return cancellationTokenSource.Token;
        }

        public KeyVaultService(SubscriptionCloudCredentials credentials)
        {
            VaultManagementClient = new KeyVaultManagementClient(credentials);
            VaultClient = new KeyVaultClient(GetKeyVaultAccessToken);
        }
        
        public async Task<KeyVault> GetKeyVault(string resourceGroup, string name, CancellationToken cancellationToken)
        {
            var vault = (await VaultManagementClient.Vaults.GetAsync(resourceGroup, name, cancellationToken)).Vault;
            return new KeyVault(VaultClient, vault);
        }

        public async Task RefreshVaults()
        {
            Vaults = new List<KeyVault>();

            var resourceGroups = await AdalHelper.Instance.GetResourceGroups();
            var resourceGroupTasks = new List<Task>();
            foreach (var group in resourceGroups)
            {
                resourceGroupTasks.Add(Task.Run(async () =>
                {
                    var response = await VaultManagementClient.Vaults.ListAsync(group, 50, KeyVaultService.GetTimeoutToken());

                    foreach (var vaultDescriptor in response.Vaults)
                    {
                        try
                        {
                            Vaults.Add(new KeyVault(VaultClient, (await VaultManagementClient.Vaults.GetAsync(group, vaultDescriptor.Name, KeyVaultService.GetTimeoutToken())).Vault));
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    string nextLink = response.NextLink;
                    while (!string.IsNullOrEmpty(response.NextLink))
                    {
                        var nextResponse = await VaultManagementClient.Vaults.ListNextAsync(nextLink, KeyVaultService.GetTimeoutToken());
                        foreach (var vaultDescriptor in nextResponse.Vaults)
                            Vaults.Add(new KeyVault(VaultClient, (await VaultManagementClient.Vaults.GetAsync(group, vaultDescriptor.Name, KeyVaultService.GetTimeoutToken())).Vault));
                        nextLink = response.NextLink;
                    }
                }));
            }
            await Task.WhenAll(resourceGroupTasks.ToArray());
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