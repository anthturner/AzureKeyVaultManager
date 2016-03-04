using System;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultServiceFactory
    {
        Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup);
        Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault);
        Task<IAzureManagementService> GetAzureManagementService();
        Task<IAzureActiveDirectoryService> GetAzureActiveDirectoryService(string tenantId);
    }
}
