using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultServiceSimulator : IKeyVaultManagementService
    {
        public async Task<ICollection<IKeyVaultSecret>> GetSecrets(IKeyVault vault)
        {
            await Task.Yield();

            return new List<IKeyVaultSecret>()
            {
                new KeyVaultSecret() { Name = "Secret One", Value = "I'm hidden!", Expires = DateTime.Now.AddDays(6), ValidAfter = DateTime.Now.AddDays(1)},
                new KeyVaultSecret() { Name = "Secret Two", Value = "p@ssW0rd1", Expires = DateTime.Now.AddDays(21)}
            };
        }
    }
}
