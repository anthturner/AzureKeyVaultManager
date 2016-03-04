using System;

namespace AzureKeyVaultManager.Contracts
{
    public interface IAzureActiveDirectoryUser
    {
        Guid ObjectId { get; set; }
        bool AccountEnabled { get; set; }
        string Mail { get; set; }
        string DisplayName { get; set; }
    }
}
