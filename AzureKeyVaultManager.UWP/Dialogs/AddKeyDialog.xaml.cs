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
    public sealed partial class AddKeyDialog : ContentDialog
    {
        private IKeyVaultKey _key;

        private enum EncryptDecryptDialogMode
        {
            Encrypt,
            Decrypt
        }

        public EncryptDecryptDialog(IKeyVaultKey key)
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
            await ExecuteTransformation(EncryptDecryptDialogMode.Encrypt);
        }

        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteTransformation(EncryptDecryptDialogMode.Decrypt);
        }

        private async Task ExecuteTransformation(EncryptDecryptDialogMode mode)
        {
            var token = await ServiceAuthentication.Authentication.Instance.GetKeyVaultApiToken(MainPage.MainPageInstance.SelectedVault.TenantId.ToString("D"));
            var vaultSvc = MainPage.MainPageInstance.Factory.GetKeyVaultService(MainPage.MainPageInstance.SelectedVault, token.AsBearer());
            
            string result = null;
            switch (mode)
            {
                case EncryptDecryptDialogMode.Encrypt:
                    string toEncrypt = null;
                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            toEncrypt = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainInputText.Text));
                            break;
                        case nameof(modeBase64String):
                            toEncrypt = base64InputText.Text;
                            break;
                        case nameof(modeFile):
                            toEncrypt = Convert.ToBase64String(File.ReadAllBytes(fileInputName.Text));
                            break;
                    }

                    result = await vaultSvc.Encrypt(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, toEncrypt);

                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            plainOutputText.Text = result;
                            break;
                        case nameof(modeBase64String):
                            base64OutputText.Text = result;
                            break;
                        case nameof(modeFile):
                            var fileOutputBytes = Convert.FromBase64String(result);
                            File.WriteAllBytes(fileOutputName.Text, fileOutputBytes);
                            break;
                    }
                    break;

                case EncryptDecryptDialogMode.Decrypt:
                    string toDecrypt = null;
                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            toDecrypt = plainInputText.Text;
                            break;
                        case nameof(modeBase64String):
                            toDecrypt = plainInputText.Text;
                            break;
                        case nameof(modeFile):
                            toDecrypt = Convert.ToBase64String(File.ReadAllBytes(fileInputName.Text));
                            break;
                    }
                    result = await vaultSvc.Decrypt(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, toDecrypt);

                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            plainOutputText.Text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result));
                            break;
                        case nameof(modeBase64String):
                            base64OutputText.Text = result;
                            break;
                        case nameof(modeFile):
                            var fileOutputBytes = Convert.FromBase64String(result);
                            File.WriteAllBytes(fileOutputName.Text, fileOutputBytes);
                            break;
                    }
                    break;
            }
        }
    }
}
