using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultSecret : KeyVaultItem
    {
        public override string Header => CurrentVersion ? Identifier.Name : Identifier.Version;
        public bool CurrentVersion { get; private set; }
        public string Value { get; private set; }

        public KeyVaultSecret(KeyVaultClient client, SecretItem secretItem, bool currentVersion) : base(client, secretItem)
        {
            CurrentVersion = currentVersion;
        }

        public KeyVaultSecret(KeyVaultClient client, Secret secretItem, bool currentVersion) : base(client, secretItem)
        {
            CurrentVersion = currentVersion;
            Value = secretItem.Value;
        }

        public override async Task Update()
        {
            await Client.UpdateSecretAsync(Identifier.Identifier, null, new SecretAttributes() { Enabled = this.Enabled, Expires = this.Expires, NotBefore = this.NotBefore }, Tags);
        }

        public override async Task Delete()
        {
            await Client.DeleteSecretAsync(Identifier.Vault, Identifier.Name);
        }
        
        public override async Task PopulateValue(string version = null, bool force = false)
        {
            if (version == null)
                version = Identifier.Version;
            if (Value != null && !force)
                return;
            try
            {
                if (Identifier.Version != null)
                    Value = (await Client.GetSecretAsync(Identifier.Vault, Identifier.Name, version)).Value;
                else
                    Value = (await Client.GetSecretAsync(Identifier.Vault, Identifier.Name)).Value;
                ValueRetrievalSuccess = true;
            }
            catch
            {
                ValueRetrievalSuccess = false;
            }
        }

        public override async Task<List<KeyVaultItem>> GetVersions()
        {
            var response = await Client.GetSecretVersionsAsync(Identifier.Vault, Identifier.Name);
            var versions = new List<SecretItem>(response.Value);

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretVersionsNextAsync(nextLink);
                versions.AddRange(nextResponse.Value);
                nextLink = response.NextLink;
            }

            return versions.Select(item => (KeyVaultItem)new KeyVaultSecret(Client, item, false)).ToList();
        }

        public async Task<KeyVaultSecret> SetValue(string value)
        {
            return new KeyVaultSecret(Client, (await Client.SetSecretAsync(Identifier.Vault, Identifier.Name, value)), true);
        }
    }
}
