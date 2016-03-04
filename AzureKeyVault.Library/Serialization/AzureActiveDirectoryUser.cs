using System;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.Serialization
{
    class AzureActiveDirectoryUser : IAzureActiveDirectoryUser
    {
        public Guid ObjectId { get; set; }
        public bool AccountEnabled { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
    }
}
