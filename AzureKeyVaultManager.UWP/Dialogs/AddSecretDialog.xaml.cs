using System;
using System.IO;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class AddSecretDialog : ContentDialog
    {
        public string SecretName { get; set; }
        public string SecretText { get; set; }

        public AddSecretDialog()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(SecretName))
            {
                secretName.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                args.Cancel = true;
            }
            if (string.IsNullOrWhiteSpace(SecretText))
            {
                secretText.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                args.Cancel = true;
            }

            if (!args.Cancel)
                Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var text = await FileOps.ReadStringFromSelectedFile();
            if (text != null)
                secretText.Text = text;
        }
    }
}
