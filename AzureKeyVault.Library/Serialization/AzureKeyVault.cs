﻿using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Serialization
{
    class AzureKeyVault : AzureResource, IKeyVault
    {
        public AzureKeyVaultProperties Properties { get; set; }
    }

    class AzureKeyVaultProperties
    {
        public AzureKeyVaultSku Sku { get; set; }
        public Guid TenantId { get; set; }
        public IEnumerable<AzureKeyVaultAccessPolicy> AccessPolicies { get; set; }
        public bool EnabledForDeployment { get; set; }
        public bool EnabledForTemplateDeployment { get; set; }
        public bool EnabledForDiskEncryption { get; set; }
        public Uri VaultUri { get; set; }
    }

    class AzureKeyVaultSku
    {
        public string Family { get; set; }
        public string Name { get; set; }
    }

    class AzureKeyVaultAccessPolicy
    {
        public Guid TenantId { get; set; }
        public Guid ObjectId { get; set; }
        public AzureKeyVaultPermissions Permissions { get; set; }
    }

    class AzureKeyVaultPermissions
    {
        public IEnumerable<String> Keys { get; set; }
        public IEnumerable<String> Secrets { get; set; }
    }
}