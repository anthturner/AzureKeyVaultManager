using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.Shared;
using System;

namespace AzureKeyVault.Connectivity.Decorators
{
    public class KeyVaultDecorator : IKeyVault
    {
        private readonly IKeyVault _vault;

        public KeyVaultDecorator(IKeyVault vault)
        {
            _vault = vault;
        }

        public virtual string Name
        {
            get { return _vault.Name; }
        }

        public virtual string ResourceGroup
        {
            get { return _vault.ResourceGroup; }
            set { }
        }

        public virtual Uri Uri
        {
            get { return _vault.Uri; }
        }

        public virtual string Id
        {
            get { return _vault.Id; }
        }

        public virtual Guid TenantId
        {
            get { return _vault.TenantId; }
        }

        public Guid SubscriptionId { get; set; }

        public KeyVaultProperties Properties
        {
            get { return _vault.Properties; }
            set { }
        }
    }
}
