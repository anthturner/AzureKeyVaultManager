using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IAzureManagementService
    {
        Task<IEnumerable<ISubscription>> GetSubscriptions();
    }
}
