using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVaultManager.UWP.Annotations;
using AzureKeyVaultManager.UWP.Commands;
using AzureKeyVaultManager.UWP.Dialogs;
using AzureKeyVaultManager.UWP.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AzureKeyVaultManager.UWP.ServiceAuthentication;
using Windows.Security.Authentication.Web.Core;
using AzureKeyVault.Connectivity.KeyVaultWrapper.Policies;

namespace AzureKeyVaultManager.UWP
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public KeyVaultViewModel SelectedVault { get; set; }
        public static IKeyVaultSecureItem SelectedKeySecret { get; private set; }
        public IKeyVaultServiceFactory Factory { get; private set; }

        public Visibility VaultSelectedVisibility { get { return SelectedVault != null ? Visibility.Visible : Visibility.Collapsed; } }
        
        private ObservableCollection<IKeyVault> vaults;
        public static MainPage MainPageInstance;

        public static Guid LoggedInOid { get; set; }

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

        public static KeyAccessPolicy SelectedVaultKeyPermissions { get; private set; }
        public static SecretAccessPolicy SelectedVaultSecretPermissions { get; private set; }

        public MainPage()
        {
            Application.Current.UnhandledException += Current_UnhandledException;
            
            MainPageInstance = this;
            
            this.InitializeComponent();
            this.DataContext = this;
        }

        private async void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var dialog = new ErrorDialog(e.Exception.ToString());
                await dialog.ShowAsync();
            }
            catch
            {
                var dialog = new Windows.UI.Popups.MessageDialog(e.Exception.ToString());
                await dialog.ShowAsync();
            }
        }

        private async Task CreateFactory()
        {
            this.Focus(FocusState.Programmatic);
            var managementApiToken = await Authentication.Instance.GetManagementApiToken();
            var graphApiToken = await Authentication.Instance.GetGraphApiToken();
            this.Factory = new AzureKeyVaultServiceFactory(managementApiToken.AsBearer(), graphApiToken.AsBearer());
            //this.Factory = new KeyVaultSimulatorFactory();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            IAzureManagementService azure;
            await ShowProgressDialog("Logging in...");
            try
            {
                await CreateFactory();
                azure = Factory.GetAzureManagementService();
            }
            catch (Exception ex)
            {
                ShowErrorDialog(new Exception("There was an error authenticating to Azure", ex).ToString());
                return;
            }

            LoggedInOid = new Guid(await Factory.GetAzureActiveDirectoryService(null).MyObjectId());

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
                            try
                            {
                                var mgmt = Factory.GetManagementService(subscription.SubscriptionId, resourceGroup.Name);
                                var localVaults = await mgmt.GetKeyVaults(new CancellationToken());

                                if (localVaults != null && localVaults.Count > 0)
                                {
                                    foreach (var vault in localVaults)
                                    {
                                        vault.ResourceGroup = resourceGroup.Name;
                                        vault.SubscriptionId = subscription.SubscriptionId;
                                        groups.Add(vault);
                                    }
                                }

                            }
                            catch { return new List<IKeyVault>(); }
                        }

                        return groups;
                    });
                    tasks.Add(t);
                }

                await Task.WhenAll(tasks);
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
                var keyVaultServiceToken = (await Authentication.Instance.GetKeyVaultApiToken(vault.TenantId.ToString("D"))).AsBearer();
                var svc = Factory.GetKeyVaultService(vault, keyVaultServiceToken);
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

                var currentUserPolicy = vault.Properties.AccessPolicies.FirstOrDefault(p => p.ObjectId == LoggedInOid);
                if (currentUserPolicy == null)
                {
                    SelectedVaultKeyPermissions = new KeyAccessPolicy();
                    SelectedVaultSecretPermissions = new SecretAccessPolicy();
                }
                else
                {
                    SelectedVaultKeyPermissions = new KeyAccessPolicy() { AccessPermissionString = currentUserPolicy.Permissions.Keys.ToArray() };
                    SelectedVaultSecretPermissions = new SecretAccessPolicy() { AccessPermissionString = currentUserPolicy.Permissions.Secrets.ToArray() };
                }

                UpdateSecrets(secretsAndKeys.Select(x =>
                {
                    if (x is IKeyVaultSecret)
                    {
                        var vm = new KeyVaultSecretViewModel((IKeyVaultSecret) x);
                        vm.ShowSecret = new ActionCommand(async () => vm.Secret = await svc.GetSecretValue((IKeyVaultSecret) x));
                        vm.SetSecret = new ActionCommand(async () => vm.Secret = await svc.SetSecretValue((IKeyVaultSecret)x, vm.Secret));
                        vm.ShowDeleteConfirmation = new ActionCommand(() => ShowItemDeleteConfirmation(x));
                        return (IKeyVaultItemViewModel) vm;
                    }
                    else if (x is IKeyVaultKey)
                    {
                        var vm = new KeyVaultKeyViewModel((IKeyVaultKey) x);
                        vm.ShowKey = new ActionCommand(async () => vm.Key = await svc.GetKeyValue((IKeyVaultKey)x));
                        vm.ShowDeleteConfirmation = new ActionCommand(() => ShowItemDeleteConfirmation(x));

                        vm.ShowEncryptDialog = new ActionCommand(() => ShowEncryptDialog((IKeyVaultKey)x));
                        vm.ShowDecryptDialog = new ActionCommand(() => ShowDecryptDialog((IKeyVaultKey)x));
                        vm.ShowSignDialog = new ActionCommand(() => ShowSignDialog((IKeyVaultKey)x));
                        vm.ShowVerifyDialog = new ActionCommand(() => ShowVerifyDialog((IKeyVaultKey)x));
                        vm.ShowWrapDialog = new ActionCommand(() => ShowWrapDialog((IKeyVaultKey)x));
                        vm.ShowUnwrapDialog = new ActionCommand(() => ShowUnwrapDialog((IKeyVaultKey)x));
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
        private async void ShowEncryptDialog(IKeyVaultKey key)
        {
            await new EncryptDecryptDialog(key).ShowAsync();
        }

        private async void ShowDecryptDialog(IKeyVaultKey key)
        {
            await new EncryptDecryptDialog(key).ShowAsync();
        }

        private async void ShowSignDialog(IKeyVaultKey key)
        {
            await new SignVerifyDialog(key).ShowAsync();
        }

        private async void ShowVerifyDialog(IKeyVaultKey key)
        {
            await new SignVerifyDialog(key).ShowAsync();
        }

        private async void ShowWrapDialog(IKeyVaultKey key)
        {
            //await new DataTransformationDialog(key, DataTransformationDialog.TransformationType.Wrap).ShowAsync();
        }

        private async void ShowUnwrapDialog(IKeyVaultKey key)
        {
            //await new DataTransformationDialog(key, DataTransformationDialog.TransformationType.Unwrap).ShowAsync();
        }


        private async void ShowAccessPermissions(IKeyVault vault)
        {
            var dialog = new KeyAccessPermissionsDialog(
                Factory.GetAzureActiveDirectoryService(vault.TenantId.ToString("D")),
                Factory.GetManagementService(vault.SubscriptionId, vault.ResourceGroup),
                vault);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // todo: save
            }
        }

        private async void ShowCreateKey(IKeyVault vault)
        {
            var dialog = new CreateKey(vault);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var svc = Factory.GetKeyVaultService(vault, (await Authentication.Instance.GetKeyVaultApiToken(vault.TenantId.ToString("D"))).AsBearer());
                
                // todo: save
            }
        }

        private async void ShowVaultDeleteConfirmation(IKeyVault vault)
        {
            var dialog = new VaultDeleteConfirmationDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var svc = Factory.GetManagementService(vault.SubscriptionId, vault.ResourceGroup);
                await svc.DeleteKeyVault(vault);
            }
        }

        private async void ShowItemDeleteConfirmation(IKeyVaultSecureItem item)
        {
            var dialog = new ItemDeleteConfirmationDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var keyVaultServiceToken = (await Authentication.Instance.GetKeyVaultApiToken(SelectedVault.TenantId.ToString("D"))).AsBearer();
                var svc = Factory.GetKeyVaultService(SelectedVault, keyVaultServiceToken);
                if (item is IKeyVaultKey)
                    await svc.Delete((IKeyVaultKey)item);
                if (item is IKeyVaultSecret)
                    await svc.Delete((IKeyVaultSecret)item);
            }
        }

        public static async void ShowErrorDialog(string text)
        {
            var dialog = new ErrorDialog(text);
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
