using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.SimulatedTypes
{
    class KeyVault : IKeyVault
    {
        public string Id { get; set; }

        public string ResourceGroup { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public Guid TenantId { get; set; }
    }
}
