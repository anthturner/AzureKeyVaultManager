using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.SimulatedTypes
{
    class KeyVaultSecret : IKeyVaultSecret
    {
        public string Name { get; set; }

        public string Value { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? ValidAfter { get; set; }
    }
}
