using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
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
