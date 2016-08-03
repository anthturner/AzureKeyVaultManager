using System;
using System.Collections.Generic;

namespace AzureKeyVault.Connectivity.Shared
{
    public class KeyVaultPermissions
    {
        public IEnumerable<String> Keys { get; set; }
        public IEnumerable<String> Secrets { get; set; }
    }
}
