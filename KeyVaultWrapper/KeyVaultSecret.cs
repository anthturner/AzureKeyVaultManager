using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultSecret : KeyVaultTreeItem
    {
        public override string Header => Name;

        public override ObservableCollection<KeyVaultTreeItem> Children
        {
            get
            {
                return new ObservableCollection<KeyVaultTreeItem>(_secrets);
            }
        }

        private KeyVaultClient Client { get; }
        public SecretItem CurrentVersion { get; }

        public string Name => CurrentVersion.Identifier.Name;

        private List<KeyVaultTreeItem> _secrets = new List<KeyVaultTreeItem>();

        internal KeyVaultSecret(KeyVaultClient client, SecretItem secret)
        {
            Client = client;
            CurrentVersion = secret;
        }

        public async Task<List<SecretItem>> GetVersions()
        {
            var response = await Client.GetSecretVersionsAsync(CurrentVersion.Identifier.Vault, Name);
            var versions = new List<SecretItem>(response.Value);

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretVersionsNextAsync(nextLink);
                versions.AddRange(nextResponse.Value);
                nextLink = response.NextLink;
            }

            _secrets = versions.Select(s => (KeyVaultTreeItem)new KeyVaultSecretVersion(Client, s)).ToList();
            var valueTasks = new List<Task>();
            foreach (KeyVaultSecretVersion secret in _secrets)
                valueTasks.Add(secret.GetValue(secret.Identifier.Version));
            await Task.WhenAll(valueTasks.ToArray());

            return versions;
        }
    }
}