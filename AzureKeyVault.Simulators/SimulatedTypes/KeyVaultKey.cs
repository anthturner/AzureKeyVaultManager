using AzureKeyVaultManager.Contracts;
using System;

namespace AzureKeyVaultManager.SimulatedTypes
{
    class KeyVaultKey : IKeyVaultKey
    {
        public string Name { get; set; }

        public string Key { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public DateTimeOffset? ValidAfter { get; set; }
    }
}
