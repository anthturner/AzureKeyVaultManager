using AzureKeyVault.Connectivity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Simulated.SimulatedTypes
{
    class KeyVaultSecret : IKeyVaultSecret
    {
        public string Name { get; set; }

        public string Value { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? ValidAfter { get; set; }
    }
}
