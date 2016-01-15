using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance { get; private set; }
        private KeyVaultService Service { get; set; }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            
            Loaded += async (sender, args) =>
            {
                var progressDialog = await this.ShowProgressAsync("Loading", "Authenticating with Azure Active Directory...");
                progressDialog.SetIndeterminate();
                
                Service = new KeyVaultService(new TokenCloudCredentials
                            (AdalHelper.Subscription, AdalHelper.Instance.GetAccessToken()));

                keyVaultTree.Service = Service;
                await keyVaultTree.Refresh(progressDialog);
                await progressDialog.CloseAsync();

                keyVaultTree.NothingSelected += (o, eventArgs) => SetDetailPane(new BlankViewer());
                keyVaultTree.VaultSelected += (o, vault) => SetDetailPane(new VaultViewer(vault));
                keyVaultTree.SecretSelected += (o, secretVersion) => SetDetailPane(new SecretViewer(secretVersion));
                keyVaultTree.KeySelected += (o, keyVersion) => SetDetailPane(new KeyViewer(keyVersion));

                keyVaultTree.SecretCreating += async (o, vault) =>
                {
                    var createDlg = new CreateSecret();
                    createDlg.Owner = this;
                    var dlg = createDlg.ShowDialog();
                    if (dlg.GetValueOrDefault(false))
                    {
                        await vault.CreateSecret(createDlg.SecretName, createDlg.Value, new SecretAttributes()
                        {
                            Enabled = createDlg.Enabled,
                            Expires = createDlg.Expires,
                            NotBefore = createDlg.NotBefore
                        });
                        var item = keyVaultTree.GetItemByContext(vault);
                        item.IsExpanded = false;
                        item.IsExpanded = true;
                    }
                };

                keyVaultTree.KeyCreating += async (o, vault) =>
                {
                    var createDlg = new CreateKey();
                    createDlg.Owner = this;
                    var dlg = createDlg.ShowDialog();
                    if (dlg.GetValueOrDefault(false))
                    {
                        await vault.CreateKey(createDlg.KeyName, createDlg.Operations, new KeyAttributes()
                        {
                            Enabled = createDlg.Enabled,
                            Expires = createDlg.Expires,
                            NotBefore = createDlg.NotBefore
                        });
                        var item = keyVaultTree.GetItemByContext(vault);
                        item.IsExpanded = false;
                        item.IsExpanded = true;
                    }
                };

                keyVaultTree.VaultDeleting += async (o, vault) =>
                {
                    var result = await this.ShowMessageAsync("Confirm", "Are you sure you want to delete this vault? ALL keys and secrets will be deleted!", MessageDialogStyle.AffirmativeAndNegative);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        await vault.Delete();
                        await keyVaultTree.Refresh();
                    }
                };

                keyVaultTree.SecretDeleting += async (o, secret) =>
                {
                    var result = await this.ShowMessageAsync("Confirm", "Are you sure you want to delete this secret? ALL versions will be deleted!", MessageDialogStyle.AffirmativeAndNegative);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        var item = keyVaultTree.GetItemByContext(keyVaultTree.GetCurrent<KeyVault>());
                        await secret.Delete();
                        item.IsExpanded = false;
                        item.Items.Clear();
                        item.Items.Add("Loading");
                        item.IsExpanded = true;
                    }
                };

                keyVaultTree.KeyDeleting += async (o, key) =>
                {
                    var result = await this.ShowMessageAsync("Confirm", "Are you sure you want to delete this key? ALL versions will be deleted!", MessageDialogStyle.AffirmativeAndNegative);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        var item = keyVaultTree.GetItemByContext(keyVaultTree.GetCurrent<KeyVault>());
                        await key.Delete();
                        item.IsExpanded = false;
                        item.Items.Clear();
                        item.Items.Add("Loading");
                        item.IsExpanded = true;
                    }
                };
            };
        }

        private void SetDetailPane(UserControl control)
        {
            detailPane.Children.Clear();
            detailPane.Children.Add(control);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            AdalHelper.Instance.ClearCache();
            Application.Current.Shutdown(0);
        }
    }
}
