using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureKeyVault.Connectivity.KeyVaultWrapper;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IKeyVaultService
    {
        Task<ICollection<IKeyVaultSecret>> GetSecrets();
        Task<String> GetSecretValue(IKeyVaultSecret secret);
        Task<String> SetSecretValue(IKeyVaultSecret secret, string value);
        Task<ICollection<IKeyVaultKey>> GetKeys();
        Task<String> GetKeyValue(IKeyVaultKey key);
        Task Delete(IKeyVaultKey key);
        Task Delete(IKeyVaultSecret secret);
        Task<string> Encrypt(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueToEncrypt);
        Task<string> Decrypt(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueToDecrypt);
        Task<string> Sign(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string digest);
        Task<bool> Verify(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string digest, string valueToVerify);
        Task<string> Wrap(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueToEncrypt);
        Task<string> Unwrap(IKeyVaultKey key, KeyVaultAlgorithm algorithm, string valueToDecrypt);
    }
}
