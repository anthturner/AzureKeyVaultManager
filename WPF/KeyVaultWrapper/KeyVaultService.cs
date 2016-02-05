using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;

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

        public KeyVaultService()
        {
            VaultManagementClient = AzureServiceAdapter.Instance.CreateKeyVaultManagementClient();
            VaultClient = AzureServiceAdapter.Instance.CreateKeyVaultClient();
        }

        public async Task CreateKeyVault(string vaultName, string resourceGroup, string location)
        {
            AccessPolicyEntry policyEntry = new AccessPolicyEntry()
            {
                PermissionsToKeys = new[] {"all"},
                PermissionsToSecrets = new[] {"all"},
                TenantId = Guid.Parse(AzureServiceAdapter.ActiveDirectoryTenantId)
            };
            var objectId = await AzureServiceAdapter.Instance.GetClientObjectId();
            if (AzureServiceAdapter.Instance.ClientAuthenticatedMode)
                policyEntry.ApplicationId = Guid.Parse(objectId);
            else
                policyEntry.ObjectId = Guid.Parse(objectId);

            await VaultManagementClient.Vaults.CreateOrUpdateAsync(resourceGroup, vaultName,
                new VaultCreateOrUpdateParameters()
                {
                    Location = location,
                    Properties = new VaultProperties()
                    {
                        EnabledForDeployment = true,
                        EnabledForTemplateDeployment = true,
                        EnabledForDiskEncryption = true,
                        Sku = new Sku { Name = "Premium", Family = "A" },
                        AccessPolicies = new List<AccessPolicyEntry>() { policyEntry },
                        TenantId = Guid.Parse(AzureServiceAdapter.ActiveDirectoryTenantId)
                    }
                });
        }

        public async Task<KeyVault> GetKeyVault(string resourceGroup, string name, CancellationToken cancellationToken)
        {
            var vault = (await VaultManagementClient.Vaults.GetAsync(resourceGroup, name, cancellationToken)).Vault;
            return new KeyVault(VaultClient, VaultManagementClient, resourceGroup, vault);
        }

        public async Task RefreshVaults()
        {
            Vaults = new List<KeyVault>();

            var resourceGroups = await AzureServiceAdapter.Instance.GetResourceGroups();
            var resourceGroupTasks = new List<Task>();
            foreach (var groupObj in resourceGroups)
            {
                resourceGroupTasks.Add(Task.Run(async () =>
                {
                    var group = groupObj.Name;
                    var response = await VaultManagementClient.Vaults.ListAsync(group, 50, GetTimeoutToken());

                    foreach (var vaultDescriptor in response.Vaults)
                    {
                        try
                        {
                            Vaults.Add(new KeyVault(VaultClient, VaultManagementClient, group, (await VaultManagementClient.Vaults.GetAsync(group, vaultDescriptor.Name, GetTimeoutToken())).Vault));
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    string nextLink = response.NextLink;
                    while (!string.IsNullOrEmpty(response.NextLink))
                    {
                        var nextResponse = await VaultManagementClient.Vaults.ListNextAsync(nextLink, GetTimeoutToken());
                        foreach (var vaultDescriptor in nextResponse.Vaults)
                            Vaults.Add(new KeyVault(VaultClient, VaultManagementClient, group, (await VaultManagementClient.Vaults.GetAsync(group, vaultDescriptor.Name, GetTimeoutToken())).Vault));
                        nextLink = response.NextLink;
                    }
                }));
            }
            await Task.WhenAll(resourceGroupTasks.ToArray());
        }
    }
}