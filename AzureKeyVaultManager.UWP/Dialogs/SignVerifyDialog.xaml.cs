using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.KeyVaultWrapper;
using AzureKeyVaultManager.UWP.ServiceAuthentication;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class SignVerifyDialog : ContentDialog
    {
        private IKeyVaultKey _key;

        private enum SignVerifyDialogMode
        {
            Sign,
            Verify
        }

        public SignVerifyDialog(IKeyVaultKey key)
        {
            _key = key;
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                var allNames = Enum.GetNames(typeof(KeyVaultAlgorithm));
                var allValues = allNames.Select(name => (KeyVaultAlgorithm)Enum.Parse(typeof(KeyVaultAlgorithm), name));
                algorithmSelection.ItemsSource = allValues.Where(alg => alg.CanSignOrVerify()).ToList();
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

        private async void signButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteTransformation(SignVerifyDialogMode.Sign);
        }

        private async void verifyButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteTransformation(SignVerifyDialogMode.Verify);
        }

        private async Task ExecuteTransformation(SignVerifyDialogMode mode)
        {
            var token = await ServiceAuthentication.Authentication.Instance.GetKeyVaultApiToken(MainPage.MainPageInstance.SelectedVault.TenantId.ToString("D"));
            var vaultSvc = MainPage.MainPageInstance.Factory.GetKeyVaultService(MainPage.MainPageInstance.SelectedVault, token.AsBearer());

            // reset coloring
            plainSignatureText.Background = null;
            base64SignatureText.Background = null;
            fileSignatureText.Background = null;

            string result = null;
            switch (mode)
            {
                case SignVerifyDialogMode.Sign:
                    byte[] digest = null;
                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            digest = GetDigest(System.Text.Encoding.UTF8.GetBytes(plainInputText.Text));
                            break;
                        case nameof(modeBase64String):
                            digest = GetDigest(Convert.FromBase64String(base64InputText.Text));
                            break;
                        case nameof(modeFile):
                            digest = GetDigest(File.ReadAllBytes(fileInputName.Text));
                            break;
                    }

                    result = await vaultSvc.Sign(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, Convert.ToBase64String(digest));

                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            plainSignatureText.Text = result;
                            break;
                        case nameof(modeBase64String):
                            base64SignatureText.Text = result;
                            break;
                        case nameof(modeFile):
                            fileSignatureText.Text = result;
                            break;
                    }
                    break;

                case SignVerifyDialogMode.Verify:
                    byte[] verifyDigest = null;
                    string toVerify = null;
                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            verifyDigest = GetDigest(System.Text.Encoding.UTF8.GetBytes(plainInputText.Text));
                            toVerify = plainSignatureText.Text;
                            break;
                        case nameof(modeBase64String):
                            verifyDigest = GetDigest(Convert.FromBase64String(plainInputText.Text));
                            toVerify = base64SignatureText.Text;
                            break;
                        case nameof(modeFile):
                            verifyDigest = GetDigest(File.ReadAllBytes(fileInputName.Text));
                            toVerify = fileSignatureText.Text;
                            break;
                    }

                    var boolResult = await vaultSvc.Verify(_key, (KeyVaultAlgorithm)algorithmSelection.SelectedItem, Convert.ToBase64String(verifyDigest), toVerify);
                    
                    switch (((PivotItem)modePivot.SelectedItem).Name)
                    {
                        case nameof(modePlainString):
                            plainSignatureText.Background = boolResult ? new SolidColorBrush(Windows.UI.Colors.Green) : new SolidColorBrush(Windows.UI.Colors.Red);
                            break;
                        case nameof(modeBase64String):
                            base64SignatureText.Background = boolResult ? new SolidColorBrush(Windows.UI.Colors.Green) : new SolidColorBrush(Windows.UI.Colors.Red);
                            break;
                        case nameof(modeFile):
                            fileSignatureText.Background = boolResult ? new SolidColorBrush(Windows.UI.Colors.Green) : new SolidColorBrush(Windows.UI.Colors.Red);
                            break;
                    }
                    break;
            }
        }

        private byte[] GetDigest(byte[] data)
        {
            var alg = (KeyVaultAlgorithm)algorithmSelection.SelectedItem;
            switch (alg)
            {
                case KeyVaultAlgorithm.RS256:
                    return System.Security.Cryptography.SHA256.Create().ComputeHash(data);
                case KeyVaultAlgorithm.RS384:
                    return System.Security.Cryptography.SHA384.Create().ComputeHash(data);
                case KeyVaultAlgorithm.RS512:
                    return System.Security.Cryptography.SHA512.Create().ComputeHash(data);
            }
            return new byte[0];
        }
    }
}