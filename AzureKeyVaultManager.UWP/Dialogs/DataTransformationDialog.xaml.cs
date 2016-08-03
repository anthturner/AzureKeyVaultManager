using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.KeyVaultWrapper;
using AzureKeyVaultManager.UWP.ServiceAuthentication;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class DataTransformationDialog : ContentDialog
    {
        public enum TransformationType
        {
            Encrypt,
            Decrypt,
            Wrap,
            Unwrap
        }

        private TransformationType _type;
        private IKeyVaultKey _key;

        public DataTransformationDialog(IKeyVaultKey key, TransformationType type)
        {
            _type = type;
            _key = key;
            this.InitializeComponent();

            this.Title = type.ToString();

            Loaded += (sender, args) =>
            {
                this.MinHeight = MainPage.MainPageInstance.ActualHeight*0.8 - 500;
                this.MinWidth = MainPage.MainPageInstance.ActualWidth*0.8;

                contentGrid.MinHeight = this.MinHeight;
            };

            var allNames = Enum.GetNames(typeof(KeyVaultAlgorithm));
            var allValues = allNames.Select(name => (KeyVaultAlgorithm)Enum.Parse(typeof(KeyVaultAlgorithm), name));
            algorithms.ItemsSource = allValues.Where(alg => alg.CanCryptOrWrap()).ToList();
        }

        private async void Forward_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch (_type)
            {
                case TransformationType.Encrypt:
                    right.Text = await ExecuteTransformation(TransformationType.Encrypt, left.Text);
                    break;
                case TransformationType.Decrypt:
                    right.Text = await ExecuteTransformation(TransformationType.Decrypt, left.Text);
                    break;
                case TransformationType.Wrap:
                    right.Text = await ExecuteTransformation(TransformationType.Wrap, left.Text);
                    break;
                case TransformationType.Unwrap:
                    right.Text = await ExecuteTransformation(TransformationType.Unwrap, left.Text);
                    break;
            }
        }

        private async void Reverse_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch (_type)
            {
                case TransformationType.Encrypt:
                    left.Text = await ExecuteTransformation(TransformationType.Decrypt, right.Text);
                    break;
                case TransformationType.Decrypt:
                    left.Text = await ExecuteTransformation(TransformationType.Encrypt, right.Text);
                    break;
                case TransformationType.Wrap:
                    left.Text = await ExecuteTransformation(TransformationType.Unwrap, right.Text);
                    break;
                case TransformationType.Unwrap:
                    left.Text = await ExecuteTransformation(TransformationType.Wrap, right.Text);
                    break;
            }
        }

        private async Task<string> ExecuteTransformation(TransformationType transformation, string value)
        {
            var token = await ServiceAuthentication.Authentication.Instance.GetKeyVaultApiToken(MainPage.MainPageInstance.SelectedVault.TenantId.ToString("D"));
            var vaultSvc = MainPage.MainPageInstance.Factory.GetKeyVaultService(MainPage.MainPageInstance.SelectedVault, token.AsBearer());
            
            switch (transformation)
            {
                case TransformationType.Encrypt:
                    return await vaultSvc.Encrypt(_key, (KeyVaultAlgorithm)algorithms.SelectedItem, left.Text);
                case TransformationType.Decrypt:
                    return await vaultSvc.Decrypt(_key, (KeyVaultAlgorithm)algorithms.SelectedItem, left.Text);
                case TransformationType.Wrap:

                    break;
                case TransformationType.Unwrap:

                    break;
            }

            await Task.Yield();
            return "";
        }
    }
}