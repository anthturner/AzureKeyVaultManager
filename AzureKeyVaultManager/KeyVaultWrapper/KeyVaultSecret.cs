using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultSecret
    {
        private KeyVaultClient Client { get; }
        private SecretItem SecretItem { get; }

        public string Name => SecretItem.Identifier.Name;
        public SecretIdentifier Identifier => SecretItem.Identifier;

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

        public KeyVaultSecret(KeyVaultClient client, SecretItem secretItem)
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
            return (await Client.GetSecretAsync(SecretItem.Identifier.Vault, Name, version)).Value;
        }

        public async Task<string> SetValue(string value)
        {
            return (await Client.SetSecretAsync(SecretItem.Identifier.Vault, Name, value)).SecretIdentifier.Version;
        }

        public async Task<List<string>> GetVersions()
        {
            // todo: change the UI to use versions across the whole pane *facepalm*
            var response = await Client.GetSecretVersionsAsync(SecretItem.Identifier.Vault, Name);
            var versions = new List<string>(response.Value.Select(s => s.Identifier.Version));

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetSecretVersionsNextAsync(nextLink);
                versions.AddRange(nextResponse.Value.Select(s => s.Identifier.Version));
                nextLink = response.NextLink;
            }
            return versions;
        }
    }
}