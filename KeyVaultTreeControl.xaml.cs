using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for KeyVaultTreeControl.xaml
    /// </summary>
    public partial class KeyVaultTreeControl : TreeView
    {
        public List<KeyVault> Vaults
        {
            get { return (List<KeyVault>) ((List<TreeViewItem>)ItemsSource).Select(src => (KeyVault)src.DataContext).ToList(); }
            set { ItemsSource = value.Select(vault => CreateTreeItem(vault, CreateVaultContextMenu())); }
        }

        public KeyVault SelectedVault => GetCurrent<KeyVault>();
        public KeyVaultSecret SelectedSecret => GetCurrent<KeyVaultSecret>();
        public KeyVaultKey SelectedKey => GetCurrent<KeyVaultKey>();
        public KeyVaultSecretVersion SelectedSecretVersion => GetCurrent<KeyVaultSecretVersion>();
        public KeyVaultKeyVersion SelectedKeyVersion => GetCurrent<KeyVaultKeyVersion>();

        public event EventHandler<KeyVault> VaultSelected;
        public event EventHandler<KeyVaultKey> KeySelected;
        public event EventHandler<KeyVaultKeyVersion> KeyVersionSelected;
        public event EventHandler<KeyVaultSecret> SecretSelected;
        public event EventHandler<KeyVaultSecretVersion> SecretVersionSelected;

        public event EventHandler<KeyVault> KeyCreating;
        public event EventHandler<KeyVault> SecretCreating;

        public event EventHandler<KeyVaultKey> KeyDeleting;
        public event EventHandler<KeyVaultSecret> SecretDeleting;

        public event EventHandler<KeyVault> VaultDeleting;

        public KeyVaultTreeControl()
        {
            InitializeComponent();
        }

        private async void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if ((item.Items.Count == 1) && (item.Items[0] is string))
            {
                item.Items.Clear();

                if (item.DataContext is KeyVault)
                {
                    await ((KeyVault) item.DataContext).ListSecrets();
                    await ((KeyVault) item.DataContext).ListKeys();

                    foreach (var vaultChild in ((KeyVault) item.DataContext).Children)
                        item.Items.Add(CreateTreeItem(vaultChild,
                            vaultChild is KeyVaultKey ? CreateKeyContextMenu() : CreateSecretContextMenu()));
                }
                else if (item.DataContext is KeyVaultSecret)
                {
                    await ((KeyVaultSecret) item.DataContext).GetVersions();
                    foreach (var vaultChild in ((KeyVaultSecret) item.DataContext).Children)
                        item.Items.Add(CreateTreeItem(vaultChild, null, true));
                }
                else if (item.DataContext is KeyVaultKey)
                {
                    await ((KeyVaultKey)item.DataContext).GetVersions();
                    foreach (var vaultChild in ((KeyVaultKey)item.DataContext).Children)
                        item.Items.Add(CreateTreeItem(vaultChild, null, true));
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
                    return GetItemByContext<T>(context);
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

        private TreeViewItem CreateTreeItem(KeyVaultTreeItem item, ContextMenu menu = null, bool omitLoading = false)
        {
            var newItem = new TreeViewItem()
            {
                Header = item.Header,
                ContextMenu = menu,
                DataContext = item
            };
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

                if (selected.DataContext is KeyVaultSecretVersion)
                    SecretVersionSelected?.Invoke(this, (KeyVaultSecretVersion)selected.DataContext);

                if (selected.DataContext is KeyVaultKey)
                    KeySelected?.Invoke(this, (KeyVaultKey)selected.DataContext);

                if (selected.DataContext is KeyVaultKeyVersion)
                    KeyVersionSelected?.Invoke(this, (KeyVaultKeyVersion)selected.DataContext);
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
    }
}
