using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Contracts
{
    public interface IKeyVault
    {
        string Name { get; }
        string ResourceGroup { get; }
        string Id { get; }
        Uri Uri { get; }
        Guid TenantId { get; }
    }
}
