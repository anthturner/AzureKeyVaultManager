using AzureKeyVaultManager.Contracts;
using System;

namespace AzureKeyVaultManager.Decorators
{
    public class KeyVaultKeyDecorator : IKeyVaultKey
    {
        private readonly IKeyVaultKey _key;

        public KeyVaultKeyDecorator(IKeyVaultKey key)
        {
            _key = key;
        }

        public virtual DateTimeOffset Created
        {
            get
            {
                return _key.Created;
            }
        }

        public virtual DateTimeOffset? Expires
        {
            get
            {
                return _key.Expires;
            }
        }

        public virtual string Name
        {
            get
            {
                return _key.Name;
            }
        }

        public virtual DateTimeOffset? Updated
        {
            get
            {
                return _key.Updated;
            }
        }

        public virtual DateTimeOffset? ValidAfter
        {
            get
            {
                return _key.ValidAfter;
            }
        }
    }
}
