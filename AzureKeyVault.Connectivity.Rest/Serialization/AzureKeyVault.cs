using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Shared;
using System;
using System.Collections.Generic;

namespace AzureKeyVault.Connectivity.Rest.Serialization
{
    class AzureKeyVault : AzureResource, IKeyVault
    {
        public KeyVaultProperties Properties { get; set; }

        public string ResourceGroup { get; set; }

        public Uri Uri
        {
            get
            {
                return this.Properties.VaultUri;
            }
        }

        public Guid TenantId
        {
            get
            {
                return this.Properties.TenantId;
            }
        }

        public Guid SubscriptionId { get; set; }
    }
}
