using AzureKeyVaultManager.Contracts;
using System;
using System.Threading.Tasks;
using AzureKeyVaultManager.UWP.ServiceAuthentication;

namespace AzureKeyVaultManager.UWP
{
    class KeyVaultServiceFactoryWithAuth : IKeyVaultServiceFactory
    {
        public async Task<IAzureManagementService> GetAzureManagementService()
        {
            var token = (await Authentication.Instance.GetManagementApiToken()).AsBearer();
            return KeyVaultManagerFactory.GetAzureManagementService(token);
        }

        public async Task<IAzureActiveDirectoryService> GetAzureActiveDirectoryService(string tenantId)
        {
            var token = (await Authentication.Instance.GetGraphApiToken()).AsBearer();
            return KeyVaultManagerFactory.GetAzureActiveDirectoryService(token, tenantId);
        }

        public async Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault)
        {
            var token = (await Authentication.Instance.GetKeyVaultApiToken(vault.TenantId.ToString("D"))).AsBearer();
            return KeyVaultManagerFactory.GetKeyVaultService(vault, token);
        }

        public async Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup)
        {
            var token = (await Authentication.Instance.GetManagementApiToken()).AsBearer();
            return KeyVaultManagerFactory.GetManagementService(subscriptionId, resourceGroup, token);
        }
    }
}
