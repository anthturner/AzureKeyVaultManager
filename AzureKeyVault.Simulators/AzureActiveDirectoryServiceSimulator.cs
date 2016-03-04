using System;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    class AzureActiveDirectoryServiceSimulator : IAzureActiveDirectoryService
    {
        public async Task<IEnumerable<IAzureActiveDirectoryUser>> SearchUsers(string searchString)
        {
            await Task.Yield();
            return new[]
            {
                new ActiveDirectoryUser()
                {
                    AccountEnabled = true,
                    DisplayName = "John Doe",
                    Mail = "john.doe@contoso.com",
                    ObjectId = Guid.NewGuid()
                }
            };
        }
    }
}
