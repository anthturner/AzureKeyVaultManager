using System;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;

namespace AzureKeyVaultManager.Serialization
{
    class AzureActiveDirectoryUser : IAzureActiveDirectoryUser
    {
        public Guid ObjectId { get; set; }
        public bool AccountEnabled { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public AccessPolicy KeyPolicy { get; set; }
        public AccessPolicy SecretPolicy { get; set; }
    }
}
