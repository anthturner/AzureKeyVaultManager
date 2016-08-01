using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IAzureManagementService
    {
        Task<IEnumerable<ISubscription>> GetSubscriptions();
        Task<IEnumerable<IResourceGroup>> GetResourceGroups(ISubscription subscription);
    }
}
