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
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Markup;
using AzureKeyVaultManager.UWP.Commands;
using AzureKeyVaultManager.UWP.ViewModels;
using AzureKeyVaultManager.UWP.Commands;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureKeyVaultManager.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ShowSecretCommand ShowSecretCommand;
        public static IKeyVaultSecret SelectedKeySecret { get; private set; }
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

        private ObservableCollection<KeyVaultSecretViewModel> originalKeysSecrets;
        private ObservableCollection<KeyVaultSecretViewModel> keysSecrets;

        public ObservableCollection<KeyVaultSecretViewModel> KeysSecrets
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
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
            UpdateSecrets(secrets.Select(x =>
            {
                var vm = new KeyVaultSecretViewModel(x);
                vm.ShowSecret = new ActionCommand(() => vm.Secret = "I'm secret");
                return vm;
            }));
        }

        private void searchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSecrets(
                from x in originalKeysSecrets
                where CultureInfo.CurrentCulture.CompareInfo.IndexOf(x.Name, searchFilter.Text, CompareOptions.IgnoreCase) >= 0
                select x);
        }

        private void UpdateSecrets(IEnumerable<KeyVaultSecretViewModel> secrets)
        {
            KeysSecrets = new ObservableCollection<KeyVaultSecretViewModel>(secrets);
        }
        private void KeysSecretsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // todo: clear value on previous SelectedKeySecret

            SelectedKeySecret = (IKeyVaultSecret)keysSecretsControl.SelectedItem;
            keysSecretsControl.ItemTemplateSelector = new CustomDataTemplateSelector();
            keysSecretsControl.UpdateLayout();
            var senderContainer = keysSecretsControl.ItemContainerGenerator.ContainerFromItem(keysSecretsControl.Items.First());
            var gridChild = FindVisualChildren<Grid>(senderContainer).First();

            gridChild.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, gridChild.ActualWidth / 10);
            gridChild.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.RowSpanProperty, gridChild.ActualHeight / 10);
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
