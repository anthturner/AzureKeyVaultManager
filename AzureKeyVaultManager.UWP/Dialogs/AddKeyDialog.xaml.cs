using AzureKeyVault.Connectivity.KeyVaultWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class AddKeyDialog : ContentDialog
    {
        public bool UsesOptions
        {
            get
            {
                return (modePivot.SelectedItem != restoreKey);
            }
        }

        public enum AddKeyDialogMode
        {
            Create,
            RSAImport,
            Restore
        }

        public AddKeyDialogMode Mode { get; set; }

        public bool Enabled { get; set; }
        public bool UseHSM { get; set; }
        public string KeyName { get; set; }
        public string ActiveAfter { get; set; }
        public string Expires { get; set; }

        public string[] KeyOps
        {
            get
            {
                var ops = new List<string>();
                if (canEncrypt.IsChecked.GetValueOrDefault(false)) ops.Add("encrypt");
                if (canDecrypt.IsChecked.GetValueOrDefault(false)) ops.Add("decrypt");
                if (canSign.IsChecked.GetValueOrDefault(false)) ops.Add("sign");
                if (canVerify.IsChecked.GetValueOrDefault(false)) ops.Add("verify");
                if (canWrap.IsChecked.GetValueOrDefault(false)) ops.Add("wrapKey");
                if (canUnwrap.IsChecked.GetValueOrDefault(false)) ops.Add("unwrapKey");
                return ops.ToArray();
            }
        }

        public string[] RSAParams
        {
            get
            {
                return new string[]
                {
                    rsaN.Text,
                    rsaE.Text,
                    rsaD.Text,
                    rsaDP.Text,
                    rsaDQ.Text,
                    rsaQI.Text,
                    rsaP.Text,
                    rsaQ.Text
                };
            }
        }

        public string RestoreKey { get; set; }

        public AddKeyDialog()
        {
            DataContext = this;
            this.InitializeComponent();
            DataContext = this;

            Loaded += (sender, args) =>
            {
                var allNames = Enum.GetNames(typeof(KeyVaultAlgorithm));
                var allValues = allNames.Select(name => (KeyVaultAlgorithm)Enum.Parse(typeof(KeyVaultAlgorithm), name));
                //algorithmSelection.ItemsSource = allValues.Where(alg => alg.CanCryptOrWrap()).ToList();
                //algorithmSelection.SelectedIndex = 0;

                //this.MinHeight = MainPage.MainPageInstance.ActualHeight * 0.8 - 500;
                //this.MinWidth = MainPage.MainPageInstance.ActualWidth * 0.8;

                //contentGrid.MinHeight = this.MinHeight;
            };

            modePivot.SelectionChanged += (sender, args) => {
                this.DataContext = null;
                this.DataContext = this;
            };
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (modePivot.SelectedItem == newKey)
                Mode = AddKeyDialogMode.Create;
            else if (modePivot.SelectedItem == importRsaKey)
                Mode = AddKeyDialogMode.RSAImport;
            else if (modePivot.SelectedItem == restoreKey)
                Mode = AddKeyDialogMode.Restore;

            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
