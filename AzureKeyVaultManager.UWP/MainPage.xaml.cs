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
using Windows.UI.Core;

namespace AzureKeyVaultManager.UWP
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public KeyVaultViewModel SelectedVault { get; set; }
        public static IKeyVaultSecureItem SelectedKeySecret { get; private set; }
        public IKeyVaultServiceFactory Factory { get; }

        public Visibility VaultSelectedVisibility { get { return SelectedVault != null ? Visibility.Visible : Visibility.Collapsed; } }
        
        private ObservableCollection<IKeyVault> vaults;
        private static MainPage MainPageInstance;

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
            MainPageInstance = this;
            this.Factory = new KeyVaultServiceFactoryWithAuth();
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            IAzureManagementService azure;
            await ShowProgressDialog("Logging in...");
            try
            {
                azure = await Factory.GetAzureManagementService();
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error authenticating to Azure", ex);
            }

            try
            {
                List<Task<IEnumerable<IKeyVault>>> tasks = new List<Task<IEnumerable<IKeyVault>>>();

                await ShowProgressDialog("Retrieving key vaults...");
                var subscriptions = await azure.GetSubscriptions();
                foreach (var subscription in subscriptions)
                {
                    var t = Task.Run<IEnumerable<IKeyVault>>(async () =>
                    {
                        List<IKeyVault> groups = new List<IKeyVault>();

                        foreach (var resourceGroup in await azure.GetResourceGroups(subscription))
                        {
                            var mgmt =
                                await Factory.GetManagementService(subscription.SubscriptionId, resourceGroup.Name);
                            var localVaults = await mgmt.GetKeyVaults(new CancellationToken());
                            
                            if (localVaults != null && localVaults.Count > 0)
                            {
                                foreach (var vault in localVaults)
                                {
                                    vault.SubscriptionId = subscription.SubscriptionId;
                                    groups.Add(vault);
                                }
                            }
                        }

                        return groups;
                    });
                    tasks.Add(t);
                }

                Task.WaitAll(tasks.ToArray());
                Vaults = new ObservableCollection<IKeyVault>(tasks.SelectMany(x => x.Result).ToList());
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.ToString());
            }

            await HideProgressDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void VaultSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count == 0)
                    return;
                var item = (IKeyVault) e.AddedItems.Single();

                await ShowProgressDialog("Getting items from vault...");

                var vault = (IKeyVault) item;
                var svc = await Factory.GetKeyVaultService(vault);
                var secrets = await svc.GetSecrets();
                var keys = await svc.GetKeys();

                var castSecrets = secrets.Select(s => (IKeyVaultSecureItem) s);
                var castKeys = keys.Select(k => (IKeyVaultSecureItem) k);
                var secretsAndKeys = castSecrets.Union(castKeys);

                SelectedVault = new KeyVaultViewModel(vault)
                {
                    ShowAccessPermissions = new ActionCommand(() => ShowAccessPermissions(vault)),
                    ShowDeleteConfirmation = new ActionCommand(() => ShowVaultDeleteConfirmation(vault))
                };
                OnPropertyChanged(nameof(VaultSelectedVisibility));
                OnPropertyChanged(nameof(SelectedVault));

                UpdateSecrets(secretsAndKeys.Select(x =>
                {
                    if (x is IKeyVaultSecret)
                    {
                        var vm = new KeyVaultSecretViewModel((IKeyVaultSecret) x);
                        vm.ShowSecret = new ActionCommand(async () => vm.Secret = await svc.GetSecretValue((IKeyVaultSecret) x));
                        vm.ShowDeleteConfirmation = new ActionCommand(() => ShowItemDeleteConfirmation(x));
                        return (IKeyVaultItemViewModel) vm;
                    }
                    else if (x is IKeyVaultKey)
                    {
                        var vm = new KeyVaultKeyViewModel((IKeyVaultKey) x);
                        vm.ShowKey = new ActionCommand(async () => vm.Key = await svc.GetKeyValue((IKeyVaultKey)x));
                        vm.ShowDeleteConfirmation = new ActionCommand(() => ShowItemDeleteConfirmation(x));
                        return (IKeyVaultItemViewModel) vm;
                    }
                    return null;
                }));
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.ToString());
            }

            await HideProgressDialog();
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
            SelectedKeySecret = (IKeyVaultSecureItem) keysSecretsControl.SelectedItem;
            keysSecretsControl.ItemTemplateSelector = new CustomDataTemplateSelector();
            keysSecretsControl.UpdateLayout();
        }

        #region Dialogs
        private async void ShowAccessPermissions(IKeyVault vault)
        {
            var dialog = new AzureKeyVaultManager.UWP.Dialogs.KeyAccessPermissionsDialog(await Factory.GetAzureActiveDirectoryService(vault.TenantId.ToString("D")));
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // save
            }
        }

        private async void ShowVaultDeleteConfirmation(IKeyVault vault)
        {
            var dialog = new AzureKeyVaultManager.UWP.Dialogs.VaultDeleteConfirmationDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var svc = await Factory.GetManagementService(vault.SubscriptionId, vault.ResourceGroup);
                await svc.DeleteKeyVault(vault);
            }
        }

        private async void ShowItemDeleteConfirmation(IKeyVaultSecureItem item)
        {
            var dialog = new AzureKeyVaultManager.UWP.Dialogs.ItemDeleteConfirmationDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var svc = await Factory.GetKeyVaultService(SelectedVault);
                if (item is IKeyVaultKey)
                    await svc.Delete((IKeyVaultKey)item);
                if (item is IKeyVaultSecret)
                    await svc.Delete((IKeyVaultSecret)item);
            }
        }

        public static async void ShowErrorDialog(string text)
        {
            var dialog = new AzureKeyVaultManager.UWP.Dialogs.ErrorDialog(text);
            await dialog.ShowAsync();
        }

        public static async Task ShowProgressDialog(string text = "Please wait...")
        {
            await MainPageInstance.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainPageInstance.progressOverlay.Visibility = Visibility.Visible;
                MainPageInstance.progressText.Text = text;
            });
        }

        public static async Task HideProgressDialog()
        {
            await MainPageInstance.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { MainPageInstance.progressOverlay.Visibility = Visibility.Collapsed; });
        }
        #endregion
    }
}
