using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.KeyVaultWrapper;
using AzureKeyVaultManager.UWP.ServiceAuthentication;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class WrapUnwrapDialog : ContentDialog
    {
        private IKeyVaultKey _key;

        private enum WrapUnwrapDialogMode
        {
            Wrap,
            Unwrap
        }

        public WrapUnwrapDialog(IKeyVaultKey key)
        {
            _key = key;
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                var allNames = Enum.GetNames(typeof(KeyVaultAlgorithm));
                var allValues = allNames.Select(name => (KeyVaultAlgorithm)Enum.Parse(typeof(KeyVaultAlgorithm), name));
                algorithmSelection.ItemsSource = allValues.Where(alg => alg.CanCryptOrWrap()).ToList();
                algorithmSelection.SelectedIndex = 0;

                //this.MinHeight = MainPage.MainPageInstance.ActualHeight * 0.8 - 500;
                //this.MinWidth = MainPage.MainPageInstance.ActualWidth * 0.8;

                //contentGrid.MinHeight = this.MinHeight;
            };
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteTransformation(WrapUnwrapDialogMode.Wrap);
        }

        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteTransformation(WrapUnwrapDialogMode.Unwrap);
        }

        private async Task ExecuteTransformation(WrapUnwrapDialogMode mode)
        {
            var token = await ServiceAuthentication.Authentication.Instance.GetKeyVaultApiToken(MainPage.MainPageInstance.SelectedVault.TenantId.ToString("D"));
            var vaultSvc = MainPage.MainPageInstance.Factory.GetKeyVaultService(MainPage.MainPageInstance.SelectedVault, token.AsBearer());
            
            string result = null;
            switch (mode)
            {
                case WrapUnwrapDialogMode.Wrap:
                    var toEncrypt = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainInputText.Text));
                    result = await vaultSvc.Wrap(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, toEncrypt);
                    plainOutputText.Text = result;
                    break;

                case WrapUnwrapDialogMode.Unwrap:
                    string toDecrypt = plainInputText.Text;
                    result = await vaultSvc.Unwrap(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, toDecrypt);
                    plainOutputText.Text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result));
                    break;
            }
        }
    }
}
