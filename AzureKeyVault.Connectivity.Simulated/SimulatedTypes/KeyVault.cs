using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Shared;
using System;

namespace AzureKeyVault.Connectivity.Simulated.SimulatedTypes
{
    class KeyVault : IKeyVault
    {
        public string Id { get; set; }

        public string ResourceGroup { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public KeyVaultProperties Properties { get; set; }

        public Guid TenantId { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
