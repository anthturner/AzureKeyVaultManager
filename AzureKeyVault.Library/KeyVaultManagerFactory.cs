using AzureKeyVaultManager.Azure;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Http;
using System;

namespace AzureKeyVaultManager
{
    public static class KeyVaultManagerFactory
    {
        public static IKeyVaultManagementService GetManagementService(Guid subscriptionId, string resourceGroup, string authToken)
        {
            return new AzureKeyVaultManagementService(subscriptionId, resourceGroup, new AuthorizedHttpClient(authToken));
        }

        public static IKeyVaultService GetKeyVaultService(IKeyVault vault, string authToken)
        {
            return new KeyVaultRestClient(vault, new AuthorizedHttpClient(authToken));
        }

        public static IAzureManagementService GetAzureManagementService(string authToken)
        {
            return new AzureRestClient(new AuthorizedHttpClient(authToken));
        }

        public static IAzureActiveDirectoryService GetAzureActiveDirectoryService(string authToken, string tenant)
        {
            return new AzureActiveDirectoryRestClient(new AuthorizedHttpClient(authToken), tenant);
        }
    }
}
