using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVault : KeyVaultTreeItem
    {
        public override string Header => Name;

        public override ObservableCollection<KeyVaultTreeItem> Children
        {
            get
            {
                return new ObservableCollection<KeyVaultTreeItem>(_secrets.Union(_keys));
            }
        }

        public string Id { get; }
        public string Name { get; }

        private KeyVaultClient Client { get; }
        private Vault Vault { get; }

        private List<KeyVaultTreeItem> _secrets = new List<KeyVaultTreeItem>();
        private List<KeyVaultTreeItem> _keys = new List<KeyVaultTreeItem>();

        internal KeyVault(KeyVaultClient client, Vault vault)
        {
            Id = vault.Id;
            Name = vault.Name;

            Client = client;
            Vault = vault;
        }

        public async Task<KeyVaultKey> GetKey(string keyName, string keyVersion = null)
        {
            var key = await Client.GetKeyAsync(Vault.Properties.VaultUri, keyName, keyVersion);
            return new KeyVaultKey(Client, new KeyItem()
            {
                Attributes = key.Attributes,
                Kid = key.KeyIdentifier.Identifier,
                Tags = key.Tags
            });
        }

        public async Task<KeyVaultSecret> GetSecret(string secretName, string secretVersion = null)
        {
            var secret = await Client.GetSecretAsync(Vault.Properties.VaultUri, secretName, secretVersion);
            return new KeyVaultSecret(Client, new SecretItem()
            {
                Attributes = secret.Attributes,
                Id = secret.SecretIdentifier.Identifier,
                Tags = secret.Tags
            });
        }

        public async Task<KeyVaultSecret> CreateSecret(string secretName, string value)
        {
            await Client.SetSecretAsync(Vault.Properties.VaultUri, secretName, value);
            return await GetSecret(secretName);
        }

        public async Task<List<KeyVaultKey>> ListKeys()
        {
            if (!HasKeyAccess("list"))
                return new List<KeyVaultKey>();

            var response = await Client.GetKeysAsync(Vault.Properties.VaultUri);
            var keys = new List<KeyVaultKey>(response.Value.Select(k => new KeyVaultKey(Client, k)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetKeysNextAsync(nextLink);
                keys.AddRange(nextResponse.Value.Select(k => new KeyVaultKey(Client, k)));
                nextLink = response.NextLink;
            }

            _keys = new List<KeyVaultTreeItem>(keys);

            return keys;
        }

        public async Task<List<KeyVaultSecret>> ListSecrets()
        {
            if (!HasSecretAccess("list"))
                return new List<KeyVaultSecret>();

            var response = await Client.GetSecretsAsync(Vault.Properties.VaultUri);
            var secrets = new List<KeyVaultSecret>(response.Value.Select(s => new KeyVaultSecret(Client, s)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretsNextAsync(nextLink);
                secrets.AddRange(nextResponse.Value.Select(s => new KeyVaultSecret(Client, s)));
                nextLink = response.NextLink;
            }

            _secrets = new List<KeyVaultTreeItem>(secrets);

            return secrets;
        }

        private bool HasKeyAccess(string accessPolicy)
        {
            return true; // stub; need to bring in permissions
            var policy = Vault.Properties.AccessPolicies.FirstOrDefault(p => p.ApplicationId == Guid.Parse(AdalHelper.KeyVaultClientId));
            if (policy == null)
                return false;
            return policy.PermissionsToKeys.Contains(accessPolicy);
        }

        private bool HasSecretAccess(string accessPolicy)
        {
            return true; // stub; need to bring in permissions
            var policy = Vault.Properties.AccessPolicies.FirstOrDefault(p => p.ApplicationId == Guid.Parse(AdalHelper.KeyVaultClientId));
            if (policy == null)
                return false;
            return policy.PermissionsToSecrets.Contains(accessPolicy);
        }
    }
}