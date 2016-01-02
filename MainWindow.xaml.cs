using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Azure;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private KeyVaultService Service { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            Loaded += async (sender, args) =>
            {
                var progressDialog = await this.ShowProgressAsync("Loading", "Authenticating with Azure Active Directory...");
                progressDialog.SetIndeterminate();
                
                Service = new KeyVaultService(new TokenCloudCredentials
                            (AdalHelper.Subscription, AdalHelper.Instance.GetAccessToken()));

                await Refresh_Click(progressDialog);
                await progressDialog.CloseAsync();

                keyVaultTree.VaultSelected += (o, vault) => SetDetailPane(new BlankViewer());
                keyVaultTree.SecretSelected += (o, secret) => SetDetailPane(new BlankViewer());
                keyVaultTree.SecretVersionSelected += (o, secretVersion) => SetDetailPane(new SecretViewer(secretVersion));
                keyVaultTree.KeySelected += (o, key) => SetDetailPane(new BlankViewer());
                keyVaultTree.KeyVersionSelected += (o, keyVersion) => SetDetailPane(new KeyViewer(keyVersion));
            };
        }

        private void SetDetailPane(UserControl control)
        {
            detailPane.Children.Clear();
            detailPane.Children.Add(control);
        }

        public void ClearActivePane()
        {
            // todo: deal with redrawing the vault list
            detailPane.Children.Clear();
            detailPane.Children.Add(new BlankViewer());
        }

        private async void CreateKey_Click(object sender, RoutedEventArgs e)
        {
            if (keyVaultTree.SelectedVault == null)
                return;

            keyVaultTree.GetItemByContext(keyVaultTree.SelectedVault).IsExpanded = false;
        }

        private async void CreateSecret_Click(object sender, RoutedEventArgs e)
        {
            if (keyVaultTree.SelectedVault == null)
                return;

            var secretName = await this.ShowInputAsync("New Secret", "Enter the name of the new secret.");
            await keyVaultTree.SelectedVault.CreateSecret(secretName, "New Secret");

            keyVaultTree.GetItemByContext(keyVaultTree.SelectedVault).IsExpanded = false;
        }
        
        private async void Refresh_Click(object sender = null, RoutedEventArgs e = null)
        {
            var progressDialog = await this.ShowProgressAsync("Loading", "");
            progressDialog.SetIndeterminate();

            await Refresh_Click(progressDialog);
            await progressDialog.CloseAsync();
        }

        private async Task Refresh_Click(ProgressDialogController controller)
        {
            detailPane.Children.Clear();
            detailPane.Children.Add(new BlankViewer());

            controller.SetMessage("Refreshing Key Vault list...");

            await Service.RefreshVaults();

            keyVaultTree.Vaults = Service.Vaults;
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            AdalHelper.Instance.ClearCache();
            Application.Current.Shutdown(0);
        }
    }
}
