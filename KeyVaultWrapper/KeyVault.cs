using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVault : KeyVaultTreeItem
    {
        public override string Header => Vault.Name;

        public override ObservableCollection<KeyVaultTreeItem> Children => new ObservableCollection<KeyVaultTreeItem>(_secrets.Union(_keys));

        private KeyVaultClient Client { get; }
        private KeyVaultManagementClient ManagementClient { get; }
        private string ResourceGroup { get; }
        private Vault Vault { get; }

        private List<KeyVaultTreeItem> _secrets = new List<KeyVaultTreeItem>();
        private List<KeyVaultTreeItem> _keys = new List<KeyVaultTreeItem>();

        internal KeyVault(KeyVaultClient client, KeyVaultManagementClient managementClient, string resourceGroup, Vault vault)
        {
            Client = client;
            ResourceGroup = resourceGroup;
            ManagementClient = managementClient;
            Vault = vault;
        }

        public async Task Delete()
        {
            await ManagementClient.Vaults.DeleteAsync(ResourceGroup, Vault.Name, CancellationToken.None);
        }

        public async Task Update()
        {
            await ManagementClient.Vaults.CreateOrUpdateAsync(ResourceGroup, Vault.Name, new VaultCreateOrUpdateParameters(Vault.Properties, Vault.Location));
        }

        public async Task<KeyVaultKey> GetKey(string keyName, string keyVersion = null)
        {
            var key = await Client.GetKeyAsync(Vault.Properties.VaultUri, keyName, keyVersion);
            return new KeyVaultKey(Client, key, keyVersion != null);
        }

        public async Task<KeyVaultSecret> GetSecret(string secretName, string secretVersion = null)
        {
            var secret = await Client.GetSecretAsync(Vault.Properties.VaultUri, secretName, secretVersion);
            return new KeyVaultSecret(Client, secret, secretVersion != null);
        }

        public async Task<KeyVaultKey> CreateKey(string keyName, KeyAccessPolicy operations, KeyAttributes attributes = null)
        {
            var key = await Client.CreateKeyAsync(Vault.Properties.VaultUri, keyName, JsonWebKeyType.Rsa, key_ops:operations.AccessPermissionString, keyAttributes: attributes);
            return new KeyVaultKey(Client, key, true);
        }

        public async Task<KeyVaultSecret> CreateSecret(string secretName, string value, SecretAttributes attributes = null)
        {
            await Client.SetSecretAsync(Vault.Properties.VaultUri, secretName, value, secretAttributes: attributes);
            return await GetSecret(secretName);
        }

        public async Task<List<KeyVaultKey>> ListKeys()
        {
            if (!await HasKeyAccess("list"))
                return new List<KeyVaultKey>();

            var response = await Client.GetKeysAsync(Vault.Properties.VaultUri);
            var keys = new List<KeyVaultKey>(response.Value.Select(k => new KeyVaultKey(Client, k, true)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetKeysNextAsync(nextLink);
                keys.AddRange(nextResponse.Value.Select(k => new KeyVaultKey(Client, k, true)));
                nextLink = response.NextLink;
            }

            _keys = new List<KeyVaultTreeItem>(keys);

            return keys;
        }

        public async Task<List<KeyVaultSecret>> ListSecrets()
        {
            if (!await HasSecretAccess("list"))
                return new List<KeyVaultSecret>();

            var response = await Client.GetSecretsAsync(Vault.Properties.VaultUri);
            var secrets = new List<KeyVaultSecret>(response.Value.Select(s => new KeyVaultSecret(Client, s, true)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretsNextAsync(nextLink);
                secrets.AddRange(nextResponse.Value.Select(s => new KeyVaultSecret(Client, s, true)));
                nextLink = response.NextLink;
            }

            _secrets = new List<KeyVaultTreeItem>(secrets);

            return secrets;
        }

        public List<AccessPolicyEntry> GetAccessPolicy()
        {
            return Vault.Properties.AccessPolicies.ToList();
        }

        public async Task UpdateAccessPolicy(List<AccessPolicyEntry> accessPolicies)
        {
            Vault.Properties.AccessPolicies = accessPolicies;

            var vault = await ManagementClient.Vaults.CreateOrUpdateAsync(ResourceGroup, Vault.Name, new VaultCreateOrUpdateParameters(Vault.Properties, Vault.Location));
            Vault.Properties = vault.Vault.Properties;
            Vault.Location = vault.Vault.Location;
        }

        private async Task<bool> HasKeyAccess(string accessPolicy)
        {
            return true;
            var objectId = await AzureServiceAdapter.Instance.GetClientObjectId();
            var policy = Vault.Properties.AccessPolicies.FirstOrDefault(p => p.ApplicationId == Guid.Parse(objectId));
            if (policy == null)
                return false;
            return policy.PermissionsToKeys.Contains(accessPolicy);
        }

        private async Task<bool> HasSecretAccess(string accessPolicy)
        {
            return true;
            var objectId = await AzureServiceAdapter.Instance.GetClientObjectId();
            var policy = Vault.Properties.AccessPolicies.FirstOrDefault(p => p.ApplicationId == Guid.Parse(objectId));
            if (policy == null)
                return false;
            return policy.PermissionsToSecrets.Contains(accessPolicy);
        }
    }
}