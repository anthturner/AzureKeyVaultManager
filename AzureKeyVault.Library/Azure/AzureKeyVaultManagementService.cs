using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AzureKeyVaultManager.Http;
using System.Net.Http;

namespace AzureKeyVaultManager.Azure
{
    class AzureKeyVaultManagementService : IKeyVaultManagementService
    {
        private readonly KeyVaultManagementRestClient _client;

        public AzureKeyVaultManagementService(Guid subscriptionId, string resourceGroupName, HttpClient client)
        {
            _client = new KeyVaultManagementRestClient(subscriptionId, resourceGroupName, client);
        }

        public Task CreateKeyVault(string vaultName, string location)
        {
            throw new NotImplementedException();
        }

        public async Task<IKeyVault> GetKeyVault(string name, CancellationToken cancellationToken)
        {
            return await _client.GetVault(name);
        }

        public async Task<ICollection<IKeyVault>> GetKeyVaults(CancellationToken cancellationToken)
        {
            try
            {
                return (await _client.GetVaults()).Select(x => x as IKeyVault).ToList();
            }
            catch
            {
                return new List<IKeyVault>();
            }
        }
    }
}
