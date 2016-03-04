using AzureKeyVaultManager.Contracts;
using System;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultSimulatorFactory : IKeyVaultServiceFactory
    {
        public Task<IAzureManagementService> GetAzureManagementService()
        {
            return Task.FromResult<IAzureManagementService>(new AzureManagementServiceSimulator());
        }

        public Task<IAzureActiveDirectoryService> GetAzureActiveDirectoryService(string tenant)
        {
            return Task.FromResult<IAzureActiveDirectoryService>(new AzureActiveDirectoryServiceSimulator());
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
