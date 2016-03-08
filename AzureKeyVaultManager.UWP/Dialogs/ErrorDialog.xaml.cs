using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        public ErrorDialog(string error)
        {
            this.InitializeComponent();

            errorText.Text = error;
        }
    }
}
