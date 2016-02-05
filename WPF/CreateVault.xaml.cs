using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.WindowsAzure.Management.Models;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for CreateVault.xaml
    /// </summary>
    public partial class CreateVault : MetroWindow
    {
        public string Name { get; set; }
        public string ResourceGroup { get; set; }
        public string Location { get; set; }

        public CreateVault()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                resourceGroup.ItemsSource = await AzureServiceAdapter.Instance.GetResourceGroups();
                //location.ItemsSource = await AdalHelper.Instance.GetLocations();
                location.IsEnabled = false;
            };

            resourceGroup.SelectionChanged += (sender, args) =>
            {
                if (args.AddedItems.Count > 0)
                {
                    var locationStr = ((ResourceGroup) args.AddedItems[0]).Location;
                    location.Items.Clear();
                    location.Items.Add(locationStr);
                    location.SelectedIndex = 0;
                }
            };
        }

        private async void Create_Clicked(object sender, RoutedEventArgs e)
        {
            if (resourceGroup.SelectedIndex == -1)
                await this.ShowMessageAsync("Error", "A resource group must be selected.");
            else if (string.IsNullOrWhiteSpace(vaultName.Text))
                await this.ShowMessageAsync("Error", "Please provide a name for the new Key Vault.");
            else
            {
                Name = vaultName.Text;
                ResourceGroup = ((ResourceGroup)resourceGroup.SelectedItem).Name;
                Location = ((ResourceGroup)resourceGroup.SelectedItem).Location;
                DialogResult = true;
                this.Close();
            }
        }
    }
}
