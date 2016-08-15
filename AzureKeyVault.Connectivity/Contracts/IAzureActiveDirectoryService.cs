using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IAzureActiveDirectoryService
    {
        Task<IEnumerable<IAzureActiveDirectoryUser>> SearchUsers(string searchString);
        Task<IEnumerable<IAzureActiveDirectoryUser>> GetUsers(string[] userIds);
        Task<string> MyObjectId();
    }
}
