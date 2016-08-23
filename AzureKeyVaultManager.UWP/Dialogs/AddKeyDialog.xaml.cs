using AzureKeyVault.Connectivity.KeyVaultWrapper;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class AddKeyDialog : ContentDialog
    {
        public bool UsesOptions
        {
            get
            {
                return (modePivot.SelectedItem != restoreKey);
            }
        }

        public AddKeyDialog()
        {
            DataContext = this;
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                var allNames = Enum.GetNames(typeof(KeyVaultAlgorithm));
                var allValues = allNames.Select(name => (KeyVaultAlgorithm)Enum.Parse(typeof(KeyVaultAlgorithm), name));
                //algorithmSelection.ItemsSource = allValues.Where(alg => alg.CanCryptOrWrap()).ToList();
                //algorithmSelection.SelectedIndex = 0;

                //this.MinHeight = MainPage.MainPageInstance.ActualHeight * 0.8 - 500;
                //this.MinWidth = MainPage.MainPageInstance.ActualWidth * 0.8;

                //contentGrid.MinHeight = this.MinHeight;
            };

            modePivot.SelectionChanged += (sender, args) => {
                this.DataContext = null;
                this.DataContext = this;
            };
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
