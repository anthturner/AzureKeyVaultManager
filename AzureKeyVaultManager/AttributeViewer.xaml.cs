using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Azure.KeyVault;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for AttributeViewer.xaml
    /// </summary>
    public partial class AttributeViewer : UserControl
    {
        private KeyAttributes _keyAttributes;
        private SecretAttributes _secretAttributes;
        private Dictionary<string, string> _tags;

        public static DependencyProperty KeyAttributesProperty = DependencyProperty.Register("KeyAttributes", typeof(KeyAttributes), typeof(AttributeViewer));
        public static DependencyProperty SecretAttributesProperty = DependencyProperty.Register("SecretAttributes", typeof(SecretAttributes), typeof(AttributeViewer));

        public KeyAttributes KeyAttributes
        {
            get { return _keyAttributes; }
            set
            {
                _keyAttributes = value;
                DrawKeyAttributes();
            }
        }

        public SecretAttributes SecretAttributes
        {
            get { return _secretAttributes; }
            set
            {
                _secretAttributes = value;
                DrawSecretAttributes();
            }
        }

        public Dictionary<string, string> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                DrawTags();
            }
        }

        public AttributeViewer()
        {
            InitializeComponent();
        }

        public void UpdateSecretAttributes()
        {
            var newAttributes = new SecretAttributes();
            DateTime newExpiry, newActiveAfter;
            if (!DateTime.TryParse(expires.Text, out newExpiry))
                throw new Exception("Expiry time is in an invalid format.");
            if (!DateTime.TryParse(activeAfter.Text, out newActiveAfter))
                throw new Exception("Active After time is in an invalid format.");
            newAttributes.Expires = newExpiry;
            newAttributes.NotBefore = newActiveAfter;
            _secretAttributes = newAttributes;
        }

        private void DrawKeyAttributes()
        {
            created.Content = KeyAttributes.Created.GetValueOrDefault().ToString("G");
            lastUpdated.Content = KeyAttributes.Updated.GetValueOrDefault().ToString("G");
            activeAfter.Text = KeyAttributes.NotBefore.GetValueOrDefault().ToString("G");
            expires.Text = KeyAttributes.Expires.GetValueOrDefault().ToString("G");
            enabled.IsChecked = KeyAttributes.Enabled;
        }

        private void DrawSecretAttributes()
        {
            created.Content = SecretAttributes.Created.GetValueOrDefault().ToString("G");
            lastUpdated.Content = SecretAttributes.Updated.GetValueOrDefault().ToString("G");
            activeAfter.Text = SecretAttributes.NotBefore.GetValueOrDefault().ToString("G");
            expires.Text = SecretAttributes.Expires.GetValueOrDefault().ToString("G");
            enabled.IsChecked = SecretAttributes.Enabled;
        }

        private void DrawTags()
        {
            if (Tags == null)
                return;

            tags.Items.Clear();
            foreach (var item in Tags)
            {
                tags.Items.Add(new ListBoxItem() {Content = string.Format("'{0}' => '{1}'", item.Key, item.Value)});
            }
        }
    }
}
