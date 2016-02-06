using AzureKeyVaultManager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Serialization
{
    class AzureKeyVaultSecret : IKeyVaultSecret
    {
        public string Id { get; set; }

        public string Name
        {
            get
            {
                return this.Id.Split(new[] { '/' }, 4, StringSplitOptions.RemoveEmptyEntries).Last();
            }
        }

        public string Value
        {
            get
            {
                return "Not Implemented";
            }
        }
    }
}
//id": "secret-identifier",
//      "contentType": "content-type", 
//      "attributes":
//      {
//        "enabled": boolean, 
//        "nbf": IntDate,
//        "exp": IntDate, 
//        "created": IntDate, 
//        "updated": IntDate
//      },
//      "tags": 
//      {
//        "name-1": "value-1", 
//        "name-2": "value-2",
//      }
//    }
