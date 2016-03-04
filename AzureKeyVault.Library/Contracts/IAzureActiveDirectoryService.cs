using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IAzureActiveDirectoryService
    {
        Task<IEnumerable<IAzureActiveDirectoryUser>> SearchUsers(string searchString);
    }
}
