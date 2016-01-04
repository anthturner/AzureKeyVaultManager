using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public abstract class KeyVaultItem : KeyVaultTreeItem
    {
        public ObjectIdentifier Identifier { get; set; }

        public bool? Enabled { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? NotBefore { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public bool ValueRetrievalSuccess { get; protected set; }

        protected KeyVaultClient Client { get; private set; }

        private KeyVaultItem(KeyVaultClient client)
        {
            Client = client;
        }

        protected KeyVaultItem(KeyVaultClient client, dynamic keySecretObject) : this(client)
        {
            if (keySecretObject is KeyBundle)
                Identifier = ((KeyBundle) keySecretObject).KeyIdentifier;
            if (keySecretObject is KeyItem)
                Identifier = ((KeyItem)keySecretObject).Identifier;
            if (keySecretObject is Secret)
                Identifier = ((Secret)keySecretObject).SecretIdentifier;
            if (keySecretObject is SecretItem)
                Identifier = ((SecretItem)keySecretObject).Identifier;
            
            SetFromRetrievedObject(keySecretObject);
        }

        protected void SetFromRetrievedObject(dynamic keySecretObject)
        {
            Enabled = keySecretObject.Attributes.Enabled;
            Created = keySecretObject.Attributes.Created;
            Expires = keySecretObject.Attributes.Expires;
            NotBefore = keySecretObject.Attributes.NotBefore;
            Updated = keySecretObject.Attributes.Updated;
            Tags = keySecretObject.Tags;
        }

        public abstract Task Update();
        public abstract Task Delete();
        public abstract Task PopulateValue(string version = null, bool force = false);
        public abstract Task<List<KeyVaultItem>> GetVersions();
    }
}
