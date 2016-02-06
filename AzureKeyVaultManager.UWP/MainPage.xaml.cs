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
using AzureKeyVaultManager;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureKeyVaultManager.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static IKeyVaultSecret SelectedKeySecret { get; private set; }

        private Timer FilterChangedTimer { get; set; }
        private bool FilterChangedTimerElapsed { get; set; }
        private string LastRequest { get; set; }

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

        private ObservableCollection<IKeyVaultSecret> originalKeysSecrets;
        private ObservableCollection<IKeyVaultSecret> keysSecrets;

        public ObservableCollection<IKeyVaultSecret> KeysSecrets
        {
            get
            {
                return keysSecrets;
            }
            set
            {
                if (originalKeysSecrets == null)
                    originalKeysSecrets = value;
                this.keysSecrets = value;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void VaultSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            var item = (IKeyVault)e.AddedItems.Single();

            var svc = new KeyVaultServiceSimulator();
            var mgmt = new KeyVaultManagementServiceSimulator();
            var vault = await mgmt.GetKeyVault(item.ResourceGroup, item.Name, CancellationToken.None);
            var secrets = await svc.GetSecrets(vault);

            KeysSecrets = new ObservableCollection<IKeyVaultSecret>(secrets);
        }

        private void ShowExpandedKey(object sender, EventArgs e)
        {
            keysSecretsControl.ItemTemplateSelector = null;
            keysSecretsControl.ItemTemplate = App.Current.Resources["ExpandedKeyTemplate"] as DataTemplate;
        }

        private void ShowExpandedSecret(object sender, EventArgs e)
        {
            keysSecretsControl.ItemTemplateSelector = null;
            keysSecretsControl.ItemTemplate = App.Current.Resources["ExpandedSecretTemplate"] as DataTemplate;
        }

        private void ResetKeySecretControl(object sender, EventArgs e)
        {
            keysSecretsControl.ItemTemplate = null;
            keysSecretsControl.ItemTemplateSelector = new CustomDataTemplateSelector();
        }

        private void searchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            KeysSecrets = new ObservableCollection<IKeyVaultSecret>(
                from x in originalKeysSecrets
                where CultureInfo.CurrentCulture.CompareInfo.IndexOf(x.Name, searchFilter.Text, CompareOptions.IgnoreCase) >= 0
                select x);
        }

        private void KeysSecretsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedKeySecret = (IKeyVaultSecret)keysSecretsControl.SelectedItem;
            keysSecretsControl.ItemTemplateSelector = new CustomDataTemplateSelector();
        }
    }
}
