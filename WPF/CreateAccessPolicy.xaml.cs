using System.Timers;
using System.Windows;
using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;
using MahApps.Metro.Controls;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Application = Microsoft.Azure.ActiveDirectory.GraphClient.Application;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for CreateAccessPolicy.xaml
    /// </summary>
    public partial class CreateAccessPolicy : MetroWindow
    {
        public string ObjectId { get; set; }
        public string ApplicationId { get; set; }
        public KeyAccessPolicy KeyAccessPolicy { get; set; }
        public SecretAccessPolicy SecretAccessPolicy { get; set; }

        private Timer FilterChangedTimer { get; }
        private bool FilterChangedTimerElapsed { get; set; }
        private string LastRequest { get; set; }

        public CreateAccessPolicy()
        {
            InitializeComponent();

            FilterChangedTimer = new Timer(1000);
            FilterChangedTimer.Elapsed += (sender, args) =>
            {
                FilterChangedTimer.Enabled = false;
                FilterChangedTimerElapsed = true;

                Dispatcher.Invoke(() =>
                {
                    if (LastRequest != searchFilter.Text)
                        FilterTextChanged(null, null);
                });
            };
            FilterChangedTimer.Enabled = true;

            Loaded += (sender, args) =>
            {
                keyGroup.DataContext = new KeyAccessPolicy();
                secretGroup.DataContext = new SecretAccessPolicy();
            };
        }
        
        private async void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!FilterChangedTimer.Enabled)
                FilterChangedTimer.Enabled = true;
            else
            {
                FilterChangedTimer.Enabled = false;
                FilterChangedTimer.Enabled = true;
                return;
            }

            LastRequest = searchFilter.Text;

            if (searchFilter.Text != null && !string.IsNullOrWhiteSpace(searchFilter.Text) && searchFilter.Text.Length > 4)
                adObjects.ItemsSource = await AzureServiceAdapter.Instance.GetAllDirectoryObjects(searchFilter.Text);
        }

        private void CreateClicked(object sender, RoutedEventArgs e)
        {
            KeyAccessPolicy = (KeyAccessPolicy) keyGroup.DataContext;
            SecretAccessPolicy = (SecretAccessPolicy)secretGroup.DataContext;

            var selectedObject = (IDirectoryObject) adObjects.SelectedItem;
            if (selectedObject is Application)
                ApplicationId = ((Application) selectedObject).AppId;
            if (selectedObject is ServicePrincipal)
                ApplicationId = ((ServicePrincipal)selectedObject).AppId;

            ObjectId = selectedObject.ObjectId;

            DialogResult = true;
            this.Close();
        }
    }
}
