using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.UWP
{
    class KeyVaultServiceFactoryWithAuth : IKeyVaultServiceFactory
    {
        public async Task<IAzureManagementService> GetAzureManagementService()
        {
            var token = (await Authentication.GetManagementApiToken()).AsBearer();
            return KeyVaultManagerFactory.GetAzureManagementService(token);
        }

        public async Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault)
        {
            var token = (await Authentication.GetToken($"https://login.microsoftonline.com/{vault.TenantId.ToString("D")}/", "https://vault.azure.net")).AsBearer();
            return KeyVaultManagerFactory.GetKeyVaultService(vault, token);
        }

        public async Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup)
        {
            var token = (await Authentication.GetManagementApiToken()).AsBearer();
            return KeyVaultManagerFactory.GetManagementService(subscriptionId, resourceGroup, token);
        }
    }
}
