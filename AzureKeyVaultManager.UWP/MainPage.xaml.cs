using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.UWP.Annotations;
using AzureKeyVaultManager.UWP.ViewControls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureKeyVaultManager.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<IKeyVault> vaults;

        public ObservableCollection<IKeyVault> Vaults
        {
            get
            {
                return vaults;
            }
            set
            {
                this.vaults = value;
                this.OnPropertyChanged();
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var mgmt = new KeyVaultManagementServiceSimulator();
            var svc = new KeyVaultServiceSimulator();
            Vaults = new ObservableCollection<IKeyVault>(await mgmt.GetKeyVaults("", CancellationToken.None));

            var vaultList = new VaultList() { VaultListSource = Vaults };
            vaultList.SelectionChanged += async (sender, vault) =>
            {
                while (mainFlow.Children.Count > 1)
                    mainFlow.Children.RemoveAt(1);
                var keysSecrets = await svc.GetSecrets(vault);
                var newPane = new KeySecretList() { KeysSecretsSource = new ObservableCollection<IKeyVaultSecret>(keysSecrets) };
                mainFlow.Children.Add(newPane);
            };
            mainFlow.Children.Add(vaultList);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
