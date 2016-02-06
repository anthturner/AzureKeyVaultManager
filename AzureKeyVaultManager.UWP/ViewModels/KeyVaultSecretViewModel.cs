using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.UWP.ViewModels
{
    class KeyVaultSecretViewModel : KeyVaultSecretDecorator
    {
        public KeyVaultSecretViewModel(IKeyVaultSecret secret)
            : base(secret)
        {
        }
    }
}
