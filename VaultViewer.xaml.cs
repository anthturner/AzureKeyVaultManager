using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AzureKeyVaultManager.KeyVaultWrapper;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for VaultViewer.xaml
    /// </summary>
    public partial class VaultViewer : UserControl
    {
        public VaultViewer(KeyVault vault)
        {
            DataContext = vault;
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                var policyItemList = vault.GetAccessPolicy().Select(p => new EntityAccessPolicy(p)).ToList();

                var tasks = new List<Task>();
                foreach (EntityAccessPolicy item in policyItemList)
                    tasks.Add(item.PopulateDirectoryInformation());
                await Task.WhenAll(tasks.ToArray());

                policyItems.ItemsSource = policyItemList;
            };

            policyItems.SelectionChanged += (sender, args) =>
            {
                var policy = ((EntityAccessPolicy)args.AddedItems[0]);
                keyGroup.DataContext = policy.KeyPolicy;
                secretGroup.DataContext = policy.SecretPolicy;
            };
        }

        private async void Update_Clicked(object sender, RoutedEventArgs e)
        {
            var vault = (KeyVault) DataContext;
            ((EntityAccessPolicy) policyItems.SelectedItem).Update();
            await vault.UpdateAccessPolicy(((List<EntityAccessPolicy>) policyItems.ItemsSource).Select(p => p.OriginalPolicyEntry).ToList());
        }
    }
}
