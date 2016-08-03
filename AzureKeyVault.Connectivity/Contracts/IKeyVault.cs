using AzureKeyVault.Connectivity.Shared;
using System;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IKeyVault
    {
        string Name { get; }
        string ResourceGroup { get; set; }
        string Id { get; }
        KeyVaultProperties Properties { get; set; }
        Uri Uri { get; }
        Guid TenantId { get; }
        Guid SubscriptionId { get; set; }
    }
}
