using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVault.Connectivity.Contracts;
using System.Linq;
using AzureKeyVault.Connectivity.KeyVaultWrapper.Policies;
using System.Collections.Generic;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class KeyAccessPermissionsDialog : ContentDialog
    {
        private IAzureActiveDirectoryService DirectoryService { get; set; }
        private IKeyVault _vault;

        public List<IAzureActiveDirectoryUser> ChangedUsers { get; private set; }

        public KeyAccessPermissionsDialog() { }
        public KeyAccessPermissionsDialog(IAzureActiveDirectoryService directoryService, IKeyVaultManagementService managementService, IKeyVault vault)
        {
            ChangedUsers = new List<IAzureActiveDirectoryUser>();
            DirectoryService = directoryService;
            _vault = vault;
            this.InitializeComponent();

            this.Loaded += async (sender, args) =>
            {
                adObjects.ItemsSource = await DirectoryService.GetUsers(_vault.Properties.AccessPolicies.Select(p => p.ObjectId.ToString()).ToArray());
            };

            adObjects.SelectionChanged += async (sender, args) =>
            {
                var selectedUser = (IAzureActiveDirectoryUser)args.AddedItems[0];
                var vaultMetadata = await managementService.GetKeyVault(vault.Name, new System.Threading.CancellationToken());

                var policies = vaultMetadata.Properties.AccessPolicies.ToList();
                var selectedPolicy = policies.FirstOrDefault(p => p.ObjectId == selectedUser.ObjectId);

                this.DataContext = new PermissionSet()
                {
                    Keys = new KeyAccessPolicy() { AccessPermissionString = selectedPolicy.Permissions.Keys.ToArray() },
                    Secrets = new SecretAccessPolicy() { AccessPermissionString = selectedPolicy.Permissions.Secrets.ToArray() }
                };
            };
        }

        private async void SearchUsers_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchFilter.Text))
            {
                var users = await DirectoryService.GetUsers(_vault.Properties.AccessPolicies.Select(p => p.ObjectId.ToString()).ToArray());
                adObjects.ItemsSource = users;
            }
            else
            {
                var users = await DirectoryService.SearchUsers(searchFilter.Text);
                adObjects.ItemsSource = users;
            }
        }
    }

    public class PermissionSet
    {
        public KeyAccessPolicy Keys { get; set; }
        public SecretAccessPolicy Secrets { get; set; }
    }
}
