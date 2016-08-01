using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Rest.Serialization
{
    class AzureResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public IDictionary<string, string> Tags { get; set; }
    }
}
