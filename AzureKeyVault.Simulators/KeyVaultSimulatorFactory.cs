using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultSimulatorFactory : IKeyVaultServiceFactory
    {
        public Task<IAzureManagementService> GetAzureManagementService()
        {
            return Task.FromResult<IAzureManagementService>(new AzureManagementServiceSimulator());
        }

        public Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault)
        {
            return Task.FromResult<IKeyVaultService>(new KeyVaultServiceSimulator());
        }

        public Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup)
        {

            return Task.FromResult<IKeyVaultManagementService>(new KeyVaultManagementServiceSimulator());
        }
    }
}
