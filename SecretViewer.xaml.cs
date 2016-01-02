using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;
using MahApps.Metro.Controls.Dialogs;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for SecretViewer.xaml
    /// </summary>
    public partial class SecretViewer : UserControl
    {
        public SecretViewer(KeyVaultSecretVersion secret)
        {
            DataContext = secret;
            InitializeComponent();
        }

        private async void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Window.GetWindow(this);
            var result = await window.ShowMessageAsync("Confirm", "Are you sure you want to delete this secret?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                //await ((KeyVaultSecret) DataContext).Delete();
                window.ClearActivePane();
            }
        }

        private async void Set_Secret(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Window.GetWindow(this);
            var result = await window.ShowMessageAsync("Confirm", "Are you sure you want to set a new default value for this secret? The old value will still be accessible via its version.", MessageDialogStyle.AffirmativeAndNegative);
            string newVersion = "";
            if (result == MessageDialogResult.Affirmative)
            {
                //newVersion = await ((KeyVaultSecret)DataContext).SetValue(value.Text);
            }

            var newItem = new ComboBoxItem() {Content = newVersion};
            //secretVersion.Items.Add(newItem);

            //secretVersion.SelectedItem = newItem;
        }

        private async void Version_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var version = (string) ((ComboBoxItem) e.AddedItems[0]).Content;
                //value.Text = await ((KeyVaultSecret) DataContext).GetValue(version);
                saveButton.IsEnabled = true;
            }
            else
            {
                value.Text = "";
                saveButton.IsEnabled = true;
            }
        }
    }
}
