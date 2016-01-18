using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AzureKeyVaultManager.KeyVaultWrapper;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for VaultViewer.xaml
    /// </summary>
    public partial class VaultViewer : UserControl
    {
        public VaultViewer(KeyVault vault)
        {
            DataContext = vault;
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                await PopulatePolicyList();
            };

            policyItems.SelectionChanged += (sender, args) =>
            {
                var policy = ((EntityAccessPolicy)args.AddedItems[0]);
                keyGroup.DataContext = policy.KeyPolicy;
                secretGroup.DataContext = policy.SecretPolicy;
            };

            policyItems.PreviewMouseRightButtonDown += (sender, args) =>
            {
                TreeViewItem treeViewItem = VisualUpwardSearch(args.OriginalSource as DependencyObject);

                if (treeViewItem != null)
                {
                    treeViewItem.Focus();
                    args.Handled = true;
                }
            };
        }

        private async Task PopulatePolicyList()
        {
            var vault = (KeyVault) DataContext;
            var policyItemList = vault.GetAccessPolicy().Select(p => new EntityAccessPolicy(p)).ToList();

            var tasks = new List<Task>();
            foreach (EntityAccessPolicy item in policyItemList)
                tasks.Add(item.PopulateDirectoryInformation());
            await Task.WhenAll(tasks.ToArray());

            policyItems.ItemsSource = policyItemList;
        }

        private async void CreateNewAccessPolicy(object sender, RoutedEventArgs routedEventArgs)
        {
            var createDlg = new CreateAccessPolicy();
            createDlg.Owner = MainWindow.Instance;
            var dlgResult = createDlg.ShowDialog();
            if (dlgResult.GetValueOrDefault(false))
            {
                var policyEntry = new AccessPolicyEntry()
                {
                    PermissionsToKeys = createDlg.KeyAccessPolicy.AccessPermissionString,
                    PermissionsToSecrets = createDlg.SecretAccessPolicy.AccessPermissionString,
                };
                policyEntry.ObjectId = Guid.Parse(createDlg.ObjectId);
                policyEntry.TenantId = Guid.Parse(AzureServiceAdapter.ActiveDirectoryTenantId);

                ((List<EntityAccessPolicy>)policyItems.ItemsSource).Add(new EntityAccessPolicy(policyEntry));

                var vault = (KeyVault)DataContext;
                await vault.UpdateAccessPolicy(((List<EntityAccessPolicy>)policyItems.ItemsSource).Select(p => p.OriginalPolicyEntry).ToList());

                await PopulatePolicyList();
            }
        }

        private async void DeleteAccessPolicy(object sender, RoutedEventArgs e)
        {
            var selectedPolicy = policyItems.SelectedItem;
            if (selectedPolicy != null)
            {
                ((List<EntityAccessPolicy>)policyItems.ItemsSource).Remove((EntityAccessPolicy)selectedPolicy);
                await PopulatePolicyList();
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private async void Update_Clicked(object sender, RoutedEventArgs e)
        {
            var vault = (KeyVault) DataContext;
            ((EntityAccessPolicy) policyItems.SelectedItem).Update();
            await vault.UpdateAccessPolicy(((List<EntityAccessPolicy>) policyItems.ItemsSource).Select(p => p.OriginalPolicyEntry).ToList());
        }
    }
}
