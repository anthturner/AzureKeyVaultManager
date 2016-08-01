using AzureKeyVault.Connectivity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Rest.Serialization
{
    class AzureKeyVaultSecret : IKeyVaultSecret
    {
        public string Id { get; set; }

        public AzureKeyVaultSecretAttributes Attributes { get; set; }

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

        public DateTimeOffset Created
        {
            get
            {
                return FromUnixTimeSeconds(this.Attributes.Created);
            }
        }

        public DateTimeOffset? Updated
        {
            get
            {
                return AsDateTime(this.Attributes.Updated);
            }
        }

        public DateTimeOffset? Expires
        {
            get
            {
                return AsDateTime(this.Attributes.Exp);
            }
        }

        public DateTimeOffset? ValidAfter
        {
            get
            {
                return AsDateTime(this.Attributes.Nbf);
            }
        }

        private DateTimeOffset? AsDateTime(int? seconds)
        {
            if (seconds.HasValue)
            {
                return FromUnixTimeSeconds(seconds.Value);
            }

            return null;
        }

        private DateTimeOffset FromUnixTimeSeconds(int unixDateTime)
        {
            // Bridge from pre-.NET 4.6
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = TimeSpan.FromSeconds(unixDateTime);
            return epoch.Add(timeSpan).ToLocalTime();
        }
    }

    class AzureKeyVaultSecretValue
    {
        public string Value { get; set; }
    }

    class AzureKeyVaultSecretAttributes
    {
        public bool Enabled { get; set; }
        public int? Nbf { get; set; }
        public int? Exp { get; set; }
        public int Created { get; set; }
        public int? Updated { get; set; }
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
