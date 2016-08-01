using System;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IKeyVaultServiceFactory
    {
        IKeyVaultManagementService GetManagementService(Guid subscriptionId, string resourceGroup);
        IKeyVaultService GetKeyVaultService(IKeyVault vault, string token = null);
        IAzureManagementService GetAzureManagementService();
        IAzureActiveDirectoryService GetAzureActiveDirectoryService(string tenantId);
    }
}
