using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultServiceFactory
    {
        Task<IKeyVaultManagementService> GetManagementService(Guid subscriptionId, string resourceGroup);
        Task<IKeyVaultService> GetKeyVaultService(IKeyVault vault);
        Task<IAzureManagementService> GetAzureManagementService();
    }
}
