using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AzureKeyVaultManager.KeyVaultWrapper;
using MahApps.Metro.Controls.Dialogs;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for KeyVaultTreeControl.xaml
    /// </summary>
    public partial class KeyVaultTreeControl : TreeView
    {
        public KeyVault SelectedVault => GetCurrent<KeyVault>();
        public KeyVaultSecret SelectedSecret => GetCurrent<KeyVaultSecret>();
        public KeyVaultKey SelectedKey => GetCurrent<KeyVaultKey>();

        public event EventHandler<KeyVault> VaultSelected;
        public event EventHandler<KeyVaultKey> KeySelected;
        public event EventHandler<KeyVaultSecret> SecretSelected;

        public event EventHandler<KeyVault> KeyCreating;
        public event EventHandler<KeyVault> SecretCreating;

        public event EventHandler<KeyVaultKey> KeyDeleting;
        public event EventHandler<KeyVaultSecret> SecretDeleting;

        public event EventHandler<KeyVault> VaultDeleting;

        public event EventHandler NothingSelected;

        public KeyVaultService Service { get; set; }

        public KeyVaultTreeControl()
        {
            InitializeComponent();

            ContextMenu = new ContextMenu();
            var secretDelete = new MenuItem() { Header = "Refresh All" };
            secretDelete.Click += async (sender, args) =>
            {
                await Refresh();
                NothingSelected?.Invoke(this, null);
            };

            ContextMenu.Items.Add(secretDelete);

            this.PreviewMouseRightButtonDown += (sender, args) =>
            {
                TreeViewItem treeViewItem = VisualUpwardSearch(args.OriginalSource as DependencyObject);

                if (treeViewItem != null)
                {
                    treeViewItem.Focus();
                    args.Handled = true;
                }
            };
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        internal async Task SetVaults(List<KeyVault> vaults)
        {
            ItemsSource = new ObservableCollection<TreeViewItem>();
            foreach (var vault in vaults)
            {
                ((ObservableCollection<TreeViewItem>)ItemsSource).Add(await CreateTreeItem(vault, CreateVaultContextMenu()));
            }
        }

        private async void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if ((item.Items.Count == 1) && (item.Items[0] is string))
            {
                item.Items.Clear();

                if (item.DataContext is KeyVault)
                {
                    try
                    {
                        await ((KeyVault) item.DataContext).ListSecrets();
                    }
                    catch (Exception ex)
                    {
                        await MainWindow.Instance.ShowMessageAsync("Error", $"Error listing secrets from vault. ({ex.Message})");
                    }

                    try
                    {
                        await ((KeyVault)item.DataContext).ListKeys();
                    }
                    catch (Exception ex)
                    {
                        await MainWindow.Instance.ShowMessageAsync("Error", $"Error listing keys from vault. ({ex.Message})");
                    }

                    foreach (var vaultChild in ((KeyVault) item.DataContext).Children)
                        item.Items.Add(await CreateTreeItem(vaultChild,
                            vaultChild is KeyVaultKey ? CreateKeyContextMenu() : CreateSecretContextMenu()));
                }
                else
                {
                    var parentKey = (KeyVaultItem) item.DataContext;
                    var versions = (await parentKey.GetVersions()).OrderBy(v => v.Created);

                    foreach (var vaultChild in versions)
                    {
                        await vaultChild.PopulateValue();
                        item.Items.Add(await CreateTreeItem(vaultChild, null, true));
                    }
                }
            }
        }

        public TreeViewItem GetItemByContext<T>(T context)
        {
            foreach (TreeViewItem item in Items)
            {
                var itemContext = item.DataContext;
                if (itemContext is T && ReferenceEquals(itemContext, context))
                    return item;
                else
                    return GetItemByContext<T>((T)itemContext);
            }
            return null;
        }

        public T GetCurrent<T>() where T : KeyVaultTreeItem
        {
            if (SelectedItem == null)
                return null;
            return GetCurrent<T>((TreeViewItem)SelectedItem);
        }

        private T GetCurrent<T>(TreeViewItem root) where T : KeyVaultTreeItem
        {
            if (root.DataContext.GetType() == typeof(T))
                return (T)root.DataContext;
            else if (root.Parent is TreeViewItem)
                return GetCurrent<T>((TreeViewItem)root.Parent);
            else
                return null;
        }

        internal async Task<TreeViewItem> CreateTreeItem(KeyVaultTreeItem item, ContextMenu menu = null, bool omitLoading = false)
        {
            var newItem = new TreeViewItem()
            {
                Header = item.Header,
                ContextMenu = menu,
                DataContext = item
            };
            
            if (item is KeyVaultItem)
                await ((KeyVaultItem) item).PopulateValue();

            if (!omitLoading) newItem.Items.Add("Loading...");
            return newItem;
        }

        private void KeyVaultTreeControl_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                var selected = (TreeViewItem) e.NewValue;

                if (selected.DataContext is KeyVault)
                    VaultSelected?.Invoke(this, (KeyVault) selected.DataContext);

                if (selected.DataContext is KeyVaultSecret)
                    SecretSelected?.Invoke(this, (KeyVaultSecret)selected.DataContext);
                
                if (selected.DataContext is KeyVaultKey)
                    KeySelected?.Invoke(this, (KeyVaultKey)selected.DataContext);
            }
        }

        private ContextMenu CreateVaultContextMenu()
        {
            var cxtMenu = new ContextMenu();

            var keyCreate = new MenuItem() {Header = "Create _Key"};
            keyCreate.Click += (sender, args) => KeyCreating?.Invoke(this, SelectedVault);
            cxtMenu.Items.Add(keyCreate);

            var secretCreate = new MenuItem() {Header = "Create _Secret"};
            secretCreate.Click += (sender, args) => SecretCreating?.Invoke(this, SelectedVault);
            cxtMenu.Items.Add(secretCreate);

            cxtMenu.Items.Add(new Separator());

            var vaultDelete = new MenuItem() {Header = "Delete _Vault"};
            vaultDelete.Click += (sender, args) => VaultDeleting?.Invoke(this, SelectedVault);
            cxtMenu.Items.Add(vaultDelete);

            return cxtMenu;
        }

        private ContextMenu CreateKeyContextMenu()
        {
            var cxtMenu = new ContextMenu();
            
            var keyDelete = new MenuItem() { Header = "Delete _Key" };
            keyDelete.Click += (sender, args) => KeyDeleting?.Invoke(this, SelectedKey);
            cxtMenu.Items.Add(keyDelete);

            return cxtMenu;
        }

        private ContextMenu CreateSecretContextMenu()
        {
            var cxtMenu = new ContextMenu();

            var secretDelete = new MenuItem() { Header = "Delete _Secret" };
            secretDelete.Click += (sender, args) => SecretDeleting?.Invoke(this, SelectedSecret);
            cxtMenu.Items.Add(secretDelete);

            return cxtMenu;
        }

        public async Task Refresh()
        {
            var progressDialog = await MainWindow.Instance.ShowProgressAsync("Loading", "");
            progressDialog.SetIndeterminate();

            await Refresh(progressDialog);
            await progressDialog.CloseAsync();

            NothingSelected?.Invoke(this, null);
        }

        public async Task Refresh(ProgressDialogController controller)
        {
            controller.SetMessage("Refreshing Key Vault list...");

            await Service.RefreshVaults();
            await SetVaults(Service.Vaults);
        }
    }
}
