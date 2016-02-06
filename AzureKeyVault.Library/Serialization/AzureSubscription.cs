using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Serialization
{
    class AzureSubscription : ISubscription
    {
        public string DisplayName { get; set; }

        public Guid SubscritionId { get; set; }
    }
}
