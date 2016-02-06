using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Decorators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AzureKeyVaultManager.UWP.ViewModels
{
    public class KeyVaultSecretViewModel : KeyVaultSecretDecorator, INotifyPropertyChanged
    {
        private string _secret;

        public KeyVaultSecretViewModel(IKeyVaultSecret secret)
            : base(secret)
        {
        }

        public ICommand ShowSecret { get; set; }

        public string Secret
        {
            get
            {
                return _secret;
            }
            set
            {
                _secret = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Secret)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
