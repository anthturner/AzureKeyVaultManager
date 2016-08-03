using System;
using System.Collections.Generic;

namespace AzureKeyVault.Connectivity.Shared
{
    public class KeyVaultProperties
    {
        public Guid TenantId { get; set; }
        public IEnumerable<KeyVaultAccessPolicy> AccessPolicies { get; set; }
        public bool EnabledForDeployment { get; set; }
        public bool EnabledForTemplateDeployment { get; set; }
        public bool EnabledForDiskEncryption { get; set; }
        public Uri VaultUri { get; set; }
    }
}
