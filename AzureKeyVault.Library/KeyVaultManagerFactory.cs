using AzureKeyVaultManager.Azure;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public static class KeyVaultManagerFactory
    {
        public static IKeyVaultManagementService GetManagementService(Guid subscriptionId, string resourceGroup, string authToken)
        {
            return new AzureKeyVaultManagementService(subscriptionId, resourceGroup, new AuthorizedHttpClient(authToken));
        }
    }
}
