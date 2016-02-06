using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface ISubscription
    {
        Guid SubscritionId { get; }
        string DisplayName { get; }
    }
}
