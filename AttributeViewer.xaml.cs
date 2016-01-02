using System;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for AttributeViewer.xaml
    /// </summary>
    public partial class AttributeViewer : UserControl
    {
        public AttributeViewer()
        {
            InitializeComponent();
        }

        private async void Update_Clicked(object sender, RoutedEventArgs e)
        {
            DateTime newExpiry, newActiveAfter;
            if (!DateTime.TryParse(expires.Text, out newExpiry))
                throw new Exception("Expiry time is in an invalid format.");
            if (!DateTime.TryParse(activeAfter.Text, out newActiveAfter))
                throw new Exception("Active After time is in an invalid format.");

            if (DataContext is KeyVaultKeyVersion)
            {
                var newAttributes = new KeyAttributes()
                {
                    Enabled = enabled.IsChecked,
                    Expires = newExpiry,
                    NotBefore = newActiveAfter
                };
                ((KeyVaultKeyVersion) DataContext).Attributes = newAttributes;
                await ((KeyVaultKeyVersion)DataContext).Update();
            }

            if (DataContext is KeyVaultSecretVersion)
            {
                var newAttributes = new SecretAttributes()
                {
                    Enabled = enabled.IsChecked,
                    Expires = newExpiry,
                    NotBefore = newActiveAfter
                };
                ((KeyVaultSecretVersion)DataContext).Attributes = newAttributes;
                await ((KeyVaultSecretVersion)DataContext).Update();
            }
        }
    }
}
