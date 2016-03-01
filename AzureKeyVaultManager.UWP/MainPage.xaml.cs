using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.UWP.Annotations;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using AzureKeyVaultManager.UWP.Commands;
using AzureKeyVaultManager.UWP.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureKeyVaultManager.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static IKeyVaultSecureItem SelectedKeySecret { get; private set; }
        public IKeyVaultServiceFactory Factory { get; }

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

        private ObservableCollection<IKeyVaultItemViewModel> originalKeysSecrets;
        private ObservableCollection<IKeyVaultItemViewModel> keysSecrets;

        public ObservableCollection<IKeyVaultItemViewModel> KeysSecrets
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
            this.Factory = new KeyVaultSimulatorFactory();
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var azure = await Factory.GetAzureManagementService();

            List<Task<IEnumerable<IKeyVault>>> tasks = new List<Task<IEnumerable<IKeyVault>>>();

            foreach (var subscription in await azure.GetSubscriptions())
            {
                var t = Task.Run<IEnumerable<IKeyVault>>(async () =>
                {
                    List<IKeyVault> groups = new List<IKeyVault>();

                    foreach (var resourceGroup in await azure.GetResourceGroups(subscription))
                    {
                        var mgmt = await Factory.GetManagementService(subscription.SubscriptionId, resourceGroup.Name);
                        var vaults = await mgmt.GetKeyVaults(new CancellationToken());

                        if (vaults != null && vaults.Count > 0)
                        {
                            groups.AddRange(vaults);
                        }
                    }

                    return groups;
                });
                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Vaults = new ObservableCollection<IKeyVault>(tasks.SelectMany(x => x.Result).ToList());
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

            var vault = (IKeyVault)item;
            var svc = await Factory.GetKeyVaultService(vault);
            var secrets = await svc.GetSecrets();
            var keys = await svc.GetKeys();

            var castSecrets = secrets.Select(s => (IKeyVaultSecureItem) s);
            var castKeys = keys.Select(k => (IKeyVaultSecureItem) k);
            var secretsAndKeys = castSecrets.Union(castKeys);

            UpdateSecrets(secretsAndKeys.Select(x =>
            {
                if (x is IKeyVaultSecret)
                {
                    var vm = new KeyVaultSecretViewModel((IKeyVaultSecret)x);
                    vm.ShowSecret = new ActionCommand(() => vm.Secret = "I'm secret");
                    return (IKeyVaultItemViewModel)vm;
                }
                else if (x is IKeyVaultKey)
                {
                    var vm = new KeyVaultKeyViewModel((IKeyVaultKey)x);
                    vm.ShowKey = new ActionCommand(() => vm.Key = "I'm a key!");
                    return (IKeyVaultItemViewModel)vm;
                }
                return null;
            }));
        }

        private void searchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSecrets(
                from x in originalKeysSecrets
                where CultureInfo.CurrentCulture.CompareInfo.IndexOf(x.Name, searchFilter.Text, CompareOptions.IgnoreCase) >= 0
                select x);
        }

        private void UpdateSecrets(IEnumerable<IKeyVaultItemViewModel> secrets)
        {
            KeysSecrets = new ObservableCollection<IKeyVaultItemViewModel>(secrets);
        }

        private void KeysSecretsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // todo: clear value on previous SelectedKeySecret

            SelectedKeySecret = (IKeyVaultSecureItem) keysSecretsControl.SelectedItem;
            keysSecretsControl.ItemTemplateSelector = new CustomDataTemplateSelector();
            keysSecretsControl.UpdateLayout();
        }

        //When you tap on backward rectangle
        private void Back_rectangle_tap(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "FlipCardBack", true);
        }

        //When you tap on front rectangle
        private void Front_rectangle_tap(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "FlipCardFront", true);
        }
    }
}
