using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Rest.Serialization
{
    public class JsonValues<T>
    {
        public IEnumerable<T> Value { get; set; }
    }
}
