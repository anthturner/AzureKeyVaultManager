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
    public sealed partial class VaultList : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<IKeyVault> SelectionChanged;
        
        private ObservableCollection<IKeyVault> vaultListSource;
        public ObservableCollection<IKeyVault> VaultListSource
        {
            get { return vaultListSource; }
            set { vaultListSource = value; OnPropertyChanged(); }
        }

        public VaultList()
        {
            this.InitializeComponent();
            this.DataContext = this;
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
                SelectionChanged?.Invoke(this, (IKeyVault)e.AddedItems.Single());
        }
    }
}
