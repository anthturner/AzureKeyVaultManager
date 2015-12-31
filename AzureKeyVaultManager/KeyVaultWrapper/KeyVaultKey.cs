using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultKey
    {
        private KeyVaultClient Client { get; }
        private KeyItem KeyItem { get; set; }
        
        public string Name => KeyItem.Identifier.Name;
        public KeyIdentifier Identifier => KeyItem.Identifier;
        public KeyAttributes Attributes => KeyItem.Attributes;

        public Dictionary<string, string> Tags => KeyItem.Tags;
        
        public KeyVaultKey(KeyVaultClient client, KeyItem keyItem)
        {
            Client = client;
            KeyItem = keyItem;
        }

        public async Task Delete()
        {
            await Client.DeleteKeyAsync(KeyItem.Identifier.Vault, KeyItem.Identifier.Name);
        }

        public async Task<byte[]> Backup()
        {
            return await Client.BackupKeyAsync(KeyItem.Identifier.Vault, Name);
        }

        public async Task Restore(byte[] backup)
        {
            var keyBundle = await Client.RestoreKeyAsync(KeyItem.Identifier.Vault, backup);
            KeyItem = new KeyItem()
            {
                Attributes = keyBundle.Attributes,
                Kid = keyBundle.KeyIdentifier.Identifier,
                Tags = keyBundle.Tags
            };
        }

        public async Task<JsonWebKey> GetValue()
        {
            return (await Client.GetKeyAsync(KeyItem.Identifier.Identifier)).Key;
        }

        public async Task<byte[]> Sign(KeyVaultAlgorithm algorithm, byte[] digest)
        {
            if (!algorithm.CanSignOrVerify())
                throw new InvalidOperationException("Cannot sign with this algorithm type.");
            return (await Client.SignAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), digest)).Result;
        }

        public async Task<bool> Verify(KeyVaultAlgorithm algorithm, byte[] digest, byte[] signature)
        {
            if (!algorithm.CanSignOrVerify())
                throw new InvalidOperationException("Cannot verify with this algorithm type.");
            return await Client.VerifyAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), digest, signature);
        }

        public async Task<byte[]> Decrypt(KeyVaultAlgorithm algorithm, byte[] cipherText)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot decrypt with this algorithm type.");
            return (await Client.DecryptAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), cipherText)).Result;
        }

        public async Task<byte[]> Encrypt(KeyVaultAlgorithm algorithm, byte[] plainText)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot encrypt with this algorithm type.");
            return (await Client.EncryptAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), plainText)).Result;
        }

        public async Task<byte[]> Wrap(KeyVaultAlgorithm algorithm, byte[] key)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot wrap with this algorithm type.");
            return (await Client.WrapKeyAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), key)).Result;
        }

        public async Task<byte[]> Unwrap(KeyVaultAlgorithm algorithm, byte[] wrappedKey)
        {
            if (!algorithm.CanCryptOrWrap())
                throw new InvalidOperationException("Cannot unwrap with this algorithm type.");
            return (await Client.UnwrapKeyAsync(KeyItem.Identifier.Identifier, algorithm.GetConfigurationString(), wrappedKey)).Result;
        }

        public async Task Update()
        {
            await Client.UpdateKeyAsync(KeyItem.Identifier.Identifier, null, Attributes, Tags);
        }
    }
}