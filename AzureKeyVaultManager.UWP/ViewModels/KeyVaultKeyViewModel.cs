using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.Decorators;
using System.ComponentModel;
using System.Windows.Input;

namespace AzureKeyVaultManager.UWP.ViewModels
{
    public class KeyVaultKeyViewModel : KeyVaultKeyDecorator, IKeyVaultItemViewModel, INotifyPropertyChanged
    {
        private string _key;

        public KeyVaultKeyViewModel(IKeyVaultKey key) : base(key)
        {
        }

        public ICommand ShowKey { get; set; }

        public ICommand ShowDeleteConfirmation { get; set; }

        public ICommand ShowEncryptDialog { get; set; }
        public ICommand ShowDecryptDialog { get; set; }

        public ICommand ShowSignDialog { get; set; }
        public ICommand ShowVerifyDialog { get; set; }

        public ICommand ShowWrapDialog { get; set; }
        public ICommand ShowUnwrapDialog { get; set; }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Key)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
