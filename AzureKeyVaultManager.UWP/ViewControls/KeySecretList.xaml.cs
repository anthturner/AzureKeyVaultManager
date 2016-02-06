using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.UWP.Annotations;

namespace AzureKeyVaultManager.UWP.ViewControls
{
    public sealed partial class KeySecretList : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<IKeyVaultSecret> SelectionChanged;
        
        public KeySecretList()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        
        private ObservableCollection<IKeyVaultSecret> keysSecretsSource;
        public ObservableCollection<IKeyVaultSecret> KeysSecretsSource
        {
            get { return keysSecretsSource; }
            set { keysSecretsSource = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SelectionChanged?.Invoke(this, (IKeyVaultSecret)e.AddedItems.Single());
        }
    }
}
