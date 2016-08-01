using AzureKeyVault.Connectivity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Simulated.SimulatedTypes
{
    class Subscription : ISubscription
    {
        public string DisplayName { get; set; }

        public Guid SubscriptionId { get; set; }
    }

    class ResourceGroup : IResourceGroup
    {
        public string Id { get; set; }

        public string Location { get; set; }

        public string Name { get; set; }
    }
}
