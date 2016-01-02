using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultKey : KeyVaultTreeItem
    {
        public override string Header => Name;

        public override ObservableCollection<KeyVaultTreeItem> Children
        {
            get
            {
                return new ObservableCollection<KeyVaultTreeItem>(_keys);
            }
        }

        private KeyVaultClient Client { get; }
        public KeyItem CurrentVersion { get; private set; }
        
        public string Name => CurrentVersion.Identifier.Name;
        public KeyIdentifier Identifier => CurrentVersion.Identifier;
        public KeyAttributes Attributes => CurrentVersion.Attributes;

        private List<KeyVaultTreeItem> _keys = null;
        
        public KeyVaultKey(KeyVaultClient client, KeyItem keyItem)
        {
            Client = client;
            CurrentVersion = keyItem;
        }

        public async Task<List<KeyItem>> GetVersions()
        {
            var response = await Client.GetKeyVersionsAsync(CurrentVersion.Identifier.Vault, Name);
            var versions = new List<KeyItem>(response.Value);

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetKeyVersionsNextAsync(nextLink);
                versions.AddRange(nextResponse.Value);
                nextLink = response.NextLink;
            }

            _keys = versions.Select(s => (KeyVaultTreeItem)new KeyVaultKeyVersion(Client, s)).ToList();

            return versions;
        }
    }
}