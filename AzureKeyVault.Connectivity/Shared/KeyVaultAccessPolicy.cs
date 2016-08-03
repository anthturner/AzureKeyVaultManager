using System;

namespace AzureKeyVault.Connectivity.Shared
{
    public class KeyVaultAccessPolicy
    {
        public Guid TenantId { get; set; }
        public Guid ObjectId { get; set; }
        public KeyVaultPermissions Permissions { get; set; }
    }
}
