using System;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;

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
                    ObjectId = Guid.NewGuid(),
                    KeyPolicy = new KeyAccessPolicy()
                    {
                        CanCreate = true,
                        CanDelete = true,
                        CanEncrypt = true,
                        CanDecrypt = true,
                        CanUpdate = true,
                        CanWrap = true,
                        CanUnwrap = true
                    },
                    SecretPolicy = new SecretAccessPolicy()
                    {
                        CanGet = true,
                        CanSet = true,
                        CanList = true
                    }
                }
            };
        }
    }
}
