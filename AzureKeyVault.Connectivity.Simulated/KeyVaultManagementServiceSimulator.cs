using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Simulated.SimulatedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultManagementServiceSimulator : IKeyVaultManagementService
    {
        private readonly IList<IKeyVault> _vaults;

        public KeyVaultManagementServiceSimulator()
        {
            _vaults = new List<IKeyVault>();
            _vaults.Add(new KeyVault() { Id = "Test", ResourceGroup = "Resources!", Name = "Test" });
        }

        public async Task CreateKeyVault(string vaultName, string location)
        {
            _vaults.Add(new KeyVault() { Id = vaultName, Name = vaultName });
            await Task.Yield();
        }

        public async Task<IKeyVault> GetKeyVault(string name, CancellationToken cancellationToken)
        {
            await Task.Yield();
            return _vaults.Where(x => String.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase)).Single();
        }

        public async Task<ICollection<IKeyVault>> GetKeyVaults(CancellationToken cancellationToken)
        {
            await Task.Yield();
            return _vaults;
        }

        public async Task DeleteKeyVault(IKeyVault vault)
        {
            await Task.Yield();
        }
    }
}
