using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IKeyVault
    {
        string Name { get; }
        string ResourceGroup { get; set; }
        string Id { get; }
        Uri Uri { get; }
        Guid TenantId { get; }
        Guid SubscriptionId { get; set; }
    }
}
