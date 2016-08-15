using System;
using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Simulated.SimulatedTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureKeyVault.Connectivity.KeyVaultWrapper.Policies;

namespace AzureKeyVaultManager
{
    class AzureActiveDirectoryServiceSimulator : IAzureActiveDirectoryService
    {
        private static Guid _generatedOid = Guid.NewGuid();

        public async Task<IEnumerable<IAzureActiveDirectoryUser>> GetUsers(string[] userIds)
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

        public async Task<string> MyObjectId()
        {
            await Task.Yield();
            return _generatedOid.ToString();
        }

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
