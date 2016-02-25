﻿using AzureKeyVaultManager.Contracts;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace AzureKeyVaultManager.Serialization
{
    class AzureKeyVaultKey : IKeyVaultKey
    {
        public string Id { get; set; }

        public AzureKeyVaultKeyAttributes Attributes { get; set; }

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
                return DateTimeOffset.FromUnixTimeSeconds(this.Attributes.Created);
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
                return DateTimeOffset.FromUnixTimeSeconds(seconds.Value);
            }

            return null;
        }
    }

    class AzureKeyVaultKeyValue
    {
        public string Key { get; set; }
    }

    class AzureKeyVaultKeyAttributes
    {
        public bool Enabled { get; set; }
        public int? Nbf { get; set; }
        public int? Exp { get; set; }
        public int Created { get; set; }
        public int? Updated { get; set; }
    }
}
//{  "key":
//  {
//    "kid":"https://demo7616.vault.azure.net/keys/mytestkey/e0ec2eb3b8764192aa1e7f20fc74dab7",
//    "kty":"RSA",


//    "key_ops":
//     [
//      "encrypt","decrypt","sign","verify","wrapKey","unwrapKey"
//     ]
//    "n":"8jfbzUQDIOo8…",
//    "e":"AQAB"
//  },
//    "attributes":
//    {
//      "enabled":false,
//      "nbf":131360185,
//      "exp":141360188,      
//      "created":121360178  
//      "updated":121360179 
//    }
//},
//  "tags:"
//  {
//    "name-1": "value-1",
//    "name-2": "value-2",
//  }
