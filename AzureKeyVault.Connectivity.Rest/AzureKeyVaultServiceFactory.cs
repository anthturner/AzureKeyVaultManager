using System;
using AzureKeyVault.Connectivity.Rest.Http;
using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Rest;

namespace AzureKeyVaultManager.UWP
{
    public class AzureKeyVaultServiceFactory : IKeyVaultServiceFactory
    {
        private string _azureManagementApiToken;
        private string _graphApiToken;
        private string _keyVaultManagementApiToken;
        
        public AzureKeyVaultServiceFactory(
            string azureManagementApiToken,
            string graphApiToken
            )
        {
            _azureManagementApiToken = azureManagementApiToken;
            _graphApiToken = graphApiToken;
        }
        
        public IAzureManagementService GetAzureManagementService()
        {
            return new AzureRestClient(new AuthorizedHttpClient(_azureManagementApiToken));
        }

        public IAzureActiveDirectoryService GetAzureActiveDirectoryService(string tenantId)
        {
            return new AzureActiveDirectoryRestClient(new AuthorizedHttpClient(_graphApiToken), tenantId);
        }

        public IKeyVaultService GetKeyVaultService(IKeyVault vault, string token = null)
        {
            if (token == null)
                throw new UnauthorizedAccessException("Token must be specified in live use");
            return new KeyVaultRestClient(vault, new AuthorizedHttpClient(token));
        }

        public IKeyVaultManagementService GetManagementService(Guid subscriptionId, string resourceGroup)
        {
            return new AzureKeyVaultManagementService(subscriptionId, resourceGroup, new AuthorizedHttpClient(_azureManagementApiToken));
        }
    }
}
