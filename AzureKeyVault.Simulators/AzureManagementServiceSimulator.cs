using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    class AzureManagementServiceSimulator : IAzureManagementService
    {
        public async Task<IEnumerable<IResourceGroup>> GetResourceGroups(ISubscription subscription)
        {
            await Task.Yield();

            return new[] { new ResourceGroup()
            {
                Id = "test",
                Name = "Another Group",
                Location = "East US"
            }};
        }

        public async Task<IEnumerable<ISubscription>> GetSubscriptions()
        {
            await Task.Yield();
            return new[] { new Subscription()
            {
                SubscriptionId = Guid.NewGuid(),
                DisplayName = "Simulated Subscription"
            }};
        }
    }
}
