using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface ISubscription
    {
        Guid SubscriptionId { get; }
        string DisplayName { get; }
    }

    public interface IResourceGroup
    {
        string Id { get; }
        string Name { get; }
        string Location { get; }
    }
}
