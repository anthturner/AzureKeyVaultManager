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
        Task CreateKeyVault(string vaultName, string location);
        Task<IKeyVault> GetKeyVault(string name, CancellationToken cancellationToken);
        Task<ICollection<IKeyVault>> GetKeyVaults(CancellationToken cancellationToken);
        Task DeleteKeyVault(IKeyVault vault);
    }
}
