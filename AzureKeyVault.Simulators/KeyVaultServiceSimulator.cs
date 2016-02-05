using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultServiceSimulator : IKeyVaultService
    {
        public async Task<ICollection<IKeyVaultSecret>> GetSecrets(IKeyVault vault)
        {
            await Task.Yield();

            return new List<IKeyVaultSecret>()
            {
                new KeyVaultSecret() { Name = "Secret One", Value = "I'm hidden!" },
                new KeyVaultSecret() { Name = "Secret Two", Value = "p@ssW0rd1" }
            };
        }
    }
}
