using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultManagementService
    {
        Task<ICollection<IKeyVaultSecret>> GetSecrets(IKeyVault vault);
    }
}
