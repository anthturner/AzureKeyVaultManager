using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVault
    {
        public string Id { get; }
        public string Name { get; }

        private KeyVaultClient Client { get; }
        private Vault Vault { get; }

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
            var response = await Client.GetKeysAsync(Vault.Properties.VaultUri);
            var keys = new List<KeyVaultKey>(response.Value.Select(k => new KeyVaultKey(Client, k)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetKeysNextAsync(nextLink);
                keys.AddRange(nextResponse.Value.Select(k => new KeyVaultKey(Client, k)));
                nextLink = response.NextLink;
            }

            return keys;
        }

        public async Task<List<KeyVaultSecret>> ListSecrets()
        {
            var response = await Client.GetSecretsAsync(Vault.Properties.VaultUri);
            var keys = new List<KeyVaultSecret>(response.Value.Select(s => new KeyVaultSecret(Client, s)));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretsNextAsync(nextLink);
                keys.AddRange(nextResponse.Value.Select(k => new KeyVaultSecret(Client, k)));
                nextLink = response.NextLink;
            }

            return keys;
        }
    }
}