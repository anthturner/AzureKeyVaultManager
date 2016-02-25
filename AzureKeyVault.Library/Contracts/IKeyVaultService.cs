using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultService
    {
        Task<ICollection<IKeyVaultSecret>> GetSecrets();
        Task<String> GetSecretValue(IKeyVaultSecret secret);
        Task<ICollection<IKeyVaultKey>> GetKeys();
        Task<String> GetKeyValue(IKeyVaultKey key);
    }
}
