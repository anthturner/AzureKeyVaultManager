using System;
using System.Windows;
using AzureKeyVaultManager.KeyVaultWrapper;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for CreateKey.xaml
    /// </summary>
    public partial class CreateKey : MetroWindow
    {
        public DateTime? NotBefore { get; private set; }
        public DateTime? Expires { get; private set; }
        public bool Enabled { get; private set; }
        public string Value { get; private set; }
        public string KeyName { get; private set; }
        public KeyAccessPolicy AccessPolicy { get; set; }

        public CreateKey()
        {
            AccessPolicy = new KeyAccessPolicy();
            InitializeComponent();
            operations.DataContext = AccessPolicy;
        }

        private async void Create_Clicked(object sender, RoutedEventArgs e)
        {
            DateTime notBeforeField, expiresField;
            if (notBefore.Text.Length > 0)
            {
                if (!DateTime.TryParse(notBefore.Text, out notBeforeField))
                {
                    await this.ShowMessageAsync("Error", "Format for 'Not Before' date/time not valid.");
                    return;
                }
                else
                    NotBefore = notBeforeField;
            }
            if (expires.Text.Length > 0)
            {
                if (!DateTime.TryParse(expires.Text, out expiresField))
                {
                    await this.ShowMessageAsync("Error", "Format for 'Expires' date/time not valid.");
                    return;
                }
                else
                    Expires = expiresField;
            }

            Enabled = enabled.IsChecked.GetValueOrDefault(false);
            KeyName = keyName.Text;
            DialogResult = true;
        }
    }
}
