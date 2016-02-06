using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(IKeyVault), typeof(VaultList), new PropertyMetadata(null));
        
        public IKeyVault Selected { get { return _selected; } set { _selected = value; } }
        private IKeyVault _selected;

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
    }
}
