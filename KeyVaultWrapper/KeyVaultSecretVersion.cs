using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultSecretVersion : KeyVaultTreeItem
    {
        public override string Header => $"[{SecretItem.Attributes.Created.GetValueOrDefault().ToString("d")}] {SecretItem.Identifier.Version}";
        public override ObservableCollection<KeyVaultTreeItem> Children => new ObservableCollection<KeyVaultTreeItem>();

        private KeyVaultClient Client { get; }
        private SecretItem SecretItem { get; }

        public string Name => SecretItem.Identifier.Name;
        public SecretIdentifier Identifier => SecretItem.Identifier;

        public string Value { get; private set; }

        public SecretAttributes Attributes
        {
            get { return SecretItem.Attributes; }
            set { SecretItem.Attributes = value; }
        }

        public Dictionary<string, string> Tags => SecretItem.Tags;

        public string ContentType
        {
            get { return SecretItem.ContentType; }
            set { SecretItem.ContentType = value; }
        }

        public KeyVaultSecretVersion(KeyVaultClient client, SecretItem secretItem)
        {
            Client = client;
            SecretItem = secretItem;
        }

        public async Task Delete()
        {
            await Client.DeleteSecretAsync(SecretItem.Identifier.Vault, Name);
        }

        public async Task Update()
        {
            await Client.UpdateSecretAsync(SecretItem.Identifier.Identifier, SecretItem.ContentType, Attributes, Tags);
        }

        public async Task<string> GetValue()
        {
            return (await Client.GetSecretAsync(SecretItem.Identifier.Identifier)).Value;
        }

        public async Task<string> GetValue(string version)
        {
            Value = (await Client.GetSecretAsync(SecretItem.Identifier.Vault, Name, version)).Value;
            return Value;
        }

        public async Task<string> SetValue(string value)
        {
            return (await Client.SetSecretAsync(SecretItem.Identifier.Vault, Name, value)).SecretIdentifier.Version;
        }
    }
}
