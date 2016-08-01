using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVault.Connectivity.Contracts;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class KeyAccessPermissionsDialog : ContentDialog
    {
        private IAzureActiveDirectoryService DirectoryService { get; set; }

        public KeyAccessPermissionsDialog() { }
        public KeyAccessPermissionsDialog(IAzureActiveDirectoryService directoryService)
        {
            DirectoryService = directoryService;
            this.InitializeComponent();

            adObjects.SelectionChanged += (sender, args) =>
            {
                //var policy = ((EntityAccessPolicy)args.AddedItems[0]);
                //keyGroup.DataContext = policy.KeyPolicy;
                //secretGroup.DataContext = policy.SecretPolicy;
            };
        }

        private async void SearchUsers_Clicked(object sender, RoutedEventArgs e)
        {
            var users = await DirectoryService.SearchUsers(searchFilter.Text);
            adObjects.ItemsSource = users;
        }
    }
}
