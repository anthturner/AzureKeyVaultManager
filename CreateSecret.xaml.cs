using System;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for CreateSecret.xaml
    /// </summary>
    public partial class CreateSecret : MetroWindow
    {
        public DateTime? NotBefore { get; private set; }
        public DateTime? Expires { get; private set; }
        public bool Enabled { get; private set; }
        public string Value { get; private set; }
        public string SecretName { get; private set; }

        public CreateSecret()
        {
            InitializeComponent();
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

            SecretName = secretName.Text;
            Enabled = enabled.IsChecked.GetValueOrDefault(false);
            Value = value.Text;
            DialogResult = true;
        }
    }
}
