using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVaultSecret
    {
        string Name { get; }
        string Value { get; }
        DateTimeOffset Created { get; }
        DateTimeOffset? Updated { get; }
        DateTimeOffset? Expires { get; }
        DateTimeOffset? ValidAfter { get; }
    }
}
