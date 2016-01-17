using System.Windows.Controls;
using AzureKeyVaultManager.KeyVaultWrapper;

namespace AzureKeyVaultManager
{
    /// <summary>
    /// Interaction logic for KeyViewer.xaml
    /// </summary>
    public partial class KeyViewer : UserControl
    {
        public KeyViewer(KeyVaultKey key)
        {
            InitializeComponent();
            DataContext = key;
        }
    }
}
