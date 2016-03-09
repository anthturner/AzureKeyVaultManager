using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.KeyVaultWrapper;

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

        public DataTransformationDialog(IKeyVaultKey key, TransformationType type)
        {
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
    }
}
