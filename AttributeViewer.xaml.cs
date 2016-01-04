using System;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;

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

            ((KeyVaultItem) DataContext).Expires = newExpiry;
            ((KeyVaultItem) DataContext).NotBefore = newActiveAfter;

            await ((KeyVaultItem) DataContext).Update();
        }
    }
}
