using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Newtonsoft.Json;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultKey : KeyVaultItem
    {
        public override string Header => CurrentVersion ? Identifier.Name : Identifier.Version;

        public bool CurrentVersion { get; private set; }
        public JsonWebKey Value { get; private set; }

        public bool CanEncrypt
        {
            get { return HasOperation(KeyOperations.Encrypt); }
            set { SetOperation(KeyOperations.Encrypt, value); }
        }
        public bool CanDecrypt
        {
            get { return HasOperation(KeyOperations.Decrypt); }
            set { SetOperation(KeyOperations.Decrypt, value); }
        }

        public bool CanWrap
        {
            get { return HasOperation(KeyOperations.Wrap); }
            set { SetOperation(KeyOperations.Wrap, value); }
        }
        public bool CanUnwrap
        {
            get { return HasOperation(KeyOperations.Unwrap); }
            set { SetOperation(KeyOperations.Unwrap, value); }
        }

        public bool CanSign
        {
            get { return HasOperation(KeyOperations.Sign); }
            set { SetOperation(KeyOperations.Sign, value); }
        }
        public bool CanVerify
        {
            get { return HasOperation(KeyOperations.Verify); }
            set { SetOperation(KeyOperations.Verify, value); }
        }

        public string JsonKey
        {
            get
            {
                if (Value == null)
                    return string.Empty;
                return JsonConvert.SerializeObject(Value, Formatting.Indented);
            }
        }

        public KeyVaultKey(KeyVaultClient client, KeyItem keyObject, bool currentVersion) : base(client, keyObject)
        {
            CurrentVersion = currentVersion;
        }

        public KeyVaultKey(KeyVaultClient client, KeyBundle keyBundleObject, bool currentVersion) : base(client, keyBundleObject)
        {
            CurrentVersion = currentVersion;
            Value = keyBundleObject.Key;
        }

        public async override Task Update()
        {
            await Client.UpdateKeyAsync(Identifier.Identifier, null, new KeyAttributes() { Enabled = this.Enabled, Expires = this.Expires, NotBefore = this.NotBefore }, Tags);
        }

        public async override Task Delete()
        {
            await Client.DeleteKeyAsync(Identifier.Vault, Identifier.Name);
        }

        public async override Task PopulateValue(string version = null, bool force = false)
        {
            if (version == null)
                version = Identifier.Version;
            if (Value != null && !force)
                return;
            try
            {
                if (Identifier.Version != null)
                    Value = (await Client.GetKeyAsync(Identifier.Vault, Identifier.Name, version)).Key;
                else
                    Value = (await Client.GetKeyAsync(Identifier.Vault, Identifier.Name)).Key;
                ValueRetrievalSuccess = true;
            }
            catch
            {
                ValueRetrievalSuccess = false;
            }
        }

        public async override Task<List<KeyVaultItem>> GetVersions()
        {
            var response = await Client.GetKeyVersionsAsync(Identifier.Vault, Identifier.Name);
            var versions = new List<KeyItem>(response.Value);

            string nextLink = response.NextLink;
            while (!string.IsNullOrEmpty(response.NextLink))
            {
                var nextResponse = await Client.GetKeyVersionsNextAsync(nextLink);
                versions.AddRange(nextResponse.Value);
                nextLink = response.NextLink;
            }

            return versions.Select(item => (KeyVaultItem)new KeyVaultKey(Client, item, false)).ToList();
        }

        public async Task<byte[]> Backup()
        {
            return await Client.BackupKeyAsync(Identifier.Vault, Identifier.Name);
        }

        public async Task Restore(byte[] backup)
        {
            var keyBundle = await Client.RestoreKeyAsync(Identifier.Vault, backup);
            SetFromRetrievedObject(keyBundle);
            Identifier = new KeyIdentifier(keyBundle.KeyIdentifier.Identifier);
        }

        public async Task<byte[]> Sign(KeyVaultAlgorithm algorithm, byte[] digest)
        {
            if (!algorithm.CanSignOrVerify())
                throw new InvalidOperationException("Cannot sign with this algorithm type.");
            return (await Client.SignAsync(Identifier.Identifier, algorithm.GetConfigurationString(), digest)).Result;
        }

        public async Task<bool> Verify(KeyVaultAlgorithm algorithm, byte[] digest, byte[] signature)
        {
            if (!algorithm.CanSignOrVerify())
                throw new InvalidOperationException("Cannot verify with this algorithm type.");
            return await Client.VerifyAsync(Identifier.Identifier, algorithm.GetConfigurationString(), digest, signature);
        }

        public async Task<byte[]> Decrypt(KeyVaultAlgorithm algorithm, byte[] cipherText)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot decrypt with this algorithm type.");
            return (await Client.DecryptAsync(Identifier.Identifier, algorithm.GetConfigurationString(), cipherText)).Result;
        }

        public async Task<byte[]> Encrypt(KeyVaultAlgorithm algorithm, byte[] plainText)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot encrypt with this algorithm type.");
            return (await Client.EncryptAsync(Identifier.Identifier, algorithm.GetConfigurationString(), plainText)).Result;
        }

        public async Task<byte[]> Wrap(KeyVaultAlgorithm algorithm, byte[] key)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot wrap with this algorithm type.");
            return (await Client.WrapKeyAsync(Identifier.Identifier, algorithm.GetConfigurationString(), key)).Result;
        }

        public async Task<byte[]> Unwrap(KeyVaultAlgorithm algorithm, byte[] wrappedKey)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot unwrap with this algorithm type.");
            return (await Client.UnwrapKeyAsync(Identifier.Identifier, algorithm.GetConfigurationString(), wrappedKey)).Result;
        }

        private bool HasOperation(KeyOperations operation)
        {
            return Value.KeyOps.Contains(GetOperationString(operation));
        }

        private void SetOperation(KeyOperations operation, bool state)
        {
            if (HasOperation(operation) && state || !HasOperation(operation) && !state)
                return;

            if (HasOperation(operation) && !state)
                Value.KeyOps = Value.KeyOps.Where(w => w != GetOperationString(operation)).ToArray();
            else if (!HasOperation(operation) && state)
                Value.KeyOps = Value.KeyOps.Union(new[] {GetOperationString(operation)}).ToArray();
        }

        private string GetOperationString(KeyOperations operation)
        {
            return Enum.GetName(typeof (KeyOperations), operation).ToLower();
        }
    }
}
