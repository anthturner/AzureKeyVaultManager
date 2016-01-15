using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            policyItems.ItemsSource = vault.GetAccessPolicy();
            policyItems.SelectionChanged += (sender, args) =>
            {
                var policy = ((AccessPolicyEntry) args.AddedItems[0]);
                keyGroup.DataContext = new AccessPolicy() {AccessPermissionString = policy.PermissionsToKeys};
                secretGroup.DataContext = new AccessPolicy() { AccessPermissionString = policy.PermissionsToSecrets };
            };
        }

        private async void Update_Clicked(object sender, RoutedEventArgs e)
        {
            var vault = (KeyVault) DataContext;

            var keyAccess = ((AccessPolicy) keyGroup.DataContext).AccessPermissionString;
            var secretAccess = ((AccessPolicy) secretGroup.DataContext).AccessPermissionString;

            ((AccessPolicyEntry) policyItems.SelectedItem).PermissionsToKeys = keyAccess;
            ((AccessPolicyEntry) policyItems.SelectedItem).PermissionsToSecrets = secretAccess;

            await vault.UpdateAccessPolicy((List<AccessPolicyEntry>) policyItems.ItemsSource);
        }
    }
}
