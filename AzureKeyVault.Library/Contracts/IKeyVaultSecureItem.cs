using System;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultSecureItem
    {
        string Name { get; }
        DateTimeOffset Created { get; }
        DateTimeOffset? Updated { get; }
        DateTimeOffset? Expires { get; }
        DateTimeOffset? ValidAfter { get; }
    }
}
