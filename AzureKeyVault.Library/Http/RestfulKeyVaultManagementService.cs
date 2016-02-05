using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AzureKeyVaultManager.Http
{
    class RestfulKeyVaultManagementService : IKeyVaultManagementService
    {
        public Task CreateKeyVault(string vaultName, string resourceGroup, string location)
        {
            throw new NotImplementedException();
        }

        public Task<IKeyVault> GetKeyVault(string resourceGroup, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<IKeyVault>> GetKeyVaults(string resourceGroup, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
