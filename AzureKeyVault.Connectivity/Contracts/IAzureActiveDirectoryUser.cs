using System;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IAzureActiveDirectoryUser : IAccessPolicies
    {
        Guid ObjectId { get; set; }
        bool AccountEnabled { get; set; }
        string Mail { get; set; }
        string DisplayName { get; set; }
    }
}
