using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Serialization
{
    public class JsonValues<T>
    {
        public IEnumerable<T> Value { get; set; }
    }
}
