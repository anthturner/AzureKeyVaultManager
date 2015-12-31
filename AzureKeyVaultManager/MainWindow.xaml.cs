using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private object VaultListPadlock = new object();
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

                vaultList.SelectedItemChanged += (o, eventArgs) =>
                {
                    if (eventArgs.NewValue == null)
                        return;

                    detailPane.Children.Clear();
                    var paneContent = ((TreeViewItem) eventArgs.NewValue).DataContext;

                    if (paneContent is KeyVaultKey)
                        detailPane.Children.Add(new KeyViewer(paneContent as KeyVaultKey));
                    else if (paneContent is KeyVaultSecret)
                        detailPane.Children.Add(new SecretViewer(paneContent as KeyVaultSecret));
                    else
                        detailPane.Children.Add(new BlankViewer());
                };
            };
        }

        private async Task<TreeViewItem> GetVaultTree(Tuple<string,string> vaultDescriptor)
        {
            var resourceGroup = vaultDescriptor.Item1;
            var vaultName = vaultDescriptor.Item2;

            try
            {
                var vault = await Service.GetKeyVault(resourceGroup, vaultName);
                var vaultRoot = new TreeViewItem() { DataContext = vault, Header = $"[{resourceGroup}]{vault.Name}" };

                var keysRoot = new TreeViewItem() { Header = "Keys" };
                var secretsRoot = new TreeViewItem() { Header = "Secrets" };

                var keys = await vault.ListKeys();
                var secrets = await vault.ListSecrets();

                foreach (var key in keys)
                    keysRoot.Items.Add(new TreeViewItem() { DataContext = key, Header = key.Name });

                foreach (var secret in secrets)
                {
                    secretsRoot.Items.Add(new TreeViewItem() { DataContext = secret, Header = secret.Name });
                }

                vaultRoot.Items.Add(keysRoot);
                vaultRoot.Items.Add(secretsRoot);
                return vaultRoot;
            }
            catch { return null; }
        }

        public void ClearActivePane()
        {
            var parent = ((TreeViewItem) vaultList.SelectedItem).Parent;
            if (parent is TreeViewItem)
            {
                ((TreeViewItem)parent).Items.Remove(vaultList.SelectedItem);
            }
            detailPane.Children.Clear();
            detailPane.Children.Add(new BlankViewer());
        }

        private async void CreateKey_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void CreateSecret_Click(object sender, RoutedEventArgs e)
        {
            if (vaultList.SelectedItem == null)
                return;

            var vaultNode = GetSelectedVault();
            if (vaultNode == null)
                return;

            var secretName = await this.ShowInputAsync("New Secret", "Enter the name of the new secret.");
            var newSecret = await ((KeyVault)vaultNode.DataContext).CreateSecret(secretName, "New Secret");
            GetSecretBranch().Items.Add(new TreeViewItem() {Header = newSecret.Name, DataContext = newSecret});
        }

        private TreeViewItem GetSelectedVault()
        {
            TreeViewItem serverParent;
            while (true)
            {
                serverParent = (TreeViewItem)CurrentNode.Parent;
                if (CurrentNode.Parent == null)
                    break;
                if (serverParent.DataContext is KeyVault)
                    break;
            }
            return serverParent;
        }

        private TreeViewItem GetKeyBranch()
        {
            if (CurrentNode.DataContext is KeyVault)
            {
                return GetFromVaultNode(CurrentNode, "Keys");
            }
            else if (CurrentNode.DataContext is KeyVaultSecret)
            {
                var vaultNode = (TreeViewItem)((TreeViewItem)CurrentNode.Parent).Parent;
                return GetFromVaultNode(vaultNode, "Keys");
            }
            else if (CurrentNode.DataContext is KeyVaultKey)
            {
                return (TreeViewItem)CurrentNode.Parent;
            }
            return null;
        }

        private TreeViewItem GetSecretBranch()
        {
            if (CurrentNode.DataContext is KeyVault)
            {
                return GetFromVaultNode(CurrentNode, "Secrets");
            }
            else if (CurrentNode.DataContext is KeyVaultSecret)
            {
                return (TreeViewItem)CurrentNode.Parent;
            }
            else if (CurrentNode.DataContext is KeyVaultKey)
            {
                var vaultNode = (TreeViewItem) ((TreeViewItem) CurrentNode.Parent).Parent;
                return GetFromVaultNode(vaultNode, "Secrets");
            }
            return null;
        }

        private TreeViewItem GetFromVaultNode(TreeViewItem vaultNode, string searchHeader)
        {
            foreach (var item in vaultNode.Items)
            {
                var header = ((TreeViewItem)item).Header;
                if (header != null && (string)header == searchHeader)
                    return (TreeViewItem)item;
            }
            return null;
        }

        private TreeViewItem CurrentNode => (TreeViewItem) vaultList.SelectedItem;

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

            var vaultNames = Service.Vaults;

            controller.SetMessage($"Refreshing Key Vault list... (0/{vaultNames.Count})");

            vaultList.Items.Clear();
            controller.SetProgress(0.0d);
            var c = 0;

            var tasks = vaultNames.Select(async vaultName =>
            {
                var vaultRoot = GetVaultTree(vaultName);

                if (await Task.WhenAny(vaultRoot, Task.Delay(new TimeSpan(0, 0, 10))) == vaultRoot)
                {
                    lock (VaultListPadlock)
                    {
                        if (vaultRoot.Result != null)
                            vaultList.Items.Add(vaultRoot.Result);
                        c++;
                        controller.SetMessage($"Refreshing Key Vault list... ({c}/{vaultNames.Count})");
                    }
                }

                controller.SetProgress((double)c / vaultNames.Count);
            });
            await Task.WhenAll(tasks);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            AdalHelper.Instance.ClearCache();
            Application.Current.Shutdown(0);
        }
    }
}
