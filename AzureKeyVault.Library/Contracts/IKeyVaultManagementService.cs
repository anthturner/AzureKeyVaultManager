using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultManagementService
    {
        Task CreateKeyVault(string vaultName, string resourceGroup, string location);
        Task<IKeyVault> GetKeyVault(string resourceGroup, string name, CancellationToken cancellationToken);
        Task<ICollection<IKeyVault>> GetKeyVaults(string resourceGroup, CancellationToken cancellationToken);
    }
}
