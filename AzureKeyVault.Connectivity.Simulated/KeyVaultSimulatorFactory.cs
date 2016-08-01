using AzureKeyVault.Connectivity.Contracts;
using System;

namespace AzureKeyVaultManager
{
    public class KeyVaultSimulatorFactory : IKeyVaultServiceFactory
    {
        public IAzureManagementService GetAzureManagementService()
        {
            return new AzureManagementServiceSimulator();
        }

        public IAzureActiveDirectoryService GetAzureActiveDirectoryService(string tenant)
        {
            return new AzureActiveDirectoryServiceSimulator();
        }

        public IKeyVaultService GetKeyVaultService(IKeyVault vault, string token = null)
        {
            return new KeyVaultServiceSimulator();
        }

        public IKeyVaultManagementService GetManagementService(Guid subscriptionId, string resourceGroup)
        {
            return new KeyVaultManagementServiceSimulator();
        }
    }
}
