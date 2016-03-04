using AzureKeyVaultManager.Contracts;
using System;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.UWP
{
    class KeyVaultServiceFactoryWithAuth : IKeyVaultServiceFactory
    {
        private string DefaultToken { get; set; }

        public async Task<IAzureManagementService> GetAzureManagementService()
        {
            await Initialize();
            return KeyVaultManagerFactory.GetAzureManagementService(DefaultToken);
        }

        public async Task<IAzureActiveDirectoryService> GetAzureActiveDirectoryService(string tenantId)
        {
            var token = (await Authentication.GetToken($"https://login.microsoftonline.com/{tenantId}/", "https://graph.windows.net/")).AsBearer();
            return KeyVaultManagerFactory.GetAzureActiveDirectoryService(token, tenantId);
        }

        public async Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault)
        {
            var token = (await Authentication.GetToken($"https://login.microsoftonline.com/{vault.TenantId.ToString("D")}/", "https://vault.azure.net")).AsBearer();
            return KeyVaultManagerFactory.GetKeyVaultService(vault, token);
        }

        public async Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup)
        {
            await Initialize();
            return KeyVaultManagerFactory.GetManagementService(subscriptionId, resourceGroup, DefaultToken);
        }

        private async Task Initialize()
        {
            if (String.IsNullOrEmpty(DefaultToken))
            {
                DefaultToken = (await Authentication.GetManagementApiToken()).AsBearer();
            }
        }
    }
}
