using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP.ViewControls
{
    public sealed partial class KeySecretList : UserControl
    {
        public event EventHandler<IKeyVaultSecret> SelectionChanged;
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(IKeyVaultSecret), typeof(KeySecretList), new PropertyMetadata(null));

        public ObservableCollection<IKeyVaultSecret> KeysSecretsSource { get; set; } 

        public IKeyVaultSecret Selected { get { return _selected; } set { _selected = value; } }
        private IKeyVaultSecret _selected;

        public KeySecretList()
        {
            KeysSecretsSource = new ObservableCollection<IKeyVaultSecret>();
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
