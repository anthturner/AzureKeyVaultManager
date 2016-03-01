using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AzureKeyVaultManager.UWP.ViewModels;

namespace AzureKeyVaultManager.UWP
{
    public sealed partial class FlippableItemTile : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TileBackgroundProperty = DependencyProperty.Register(
            "TileBackground", typeof (Brush), typeof (FlippableItemTile), null);
        public Brush TileBackground
        {
            get { return _background; }
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        public static readonly DependencyProperty KeyVaultItemProperty = DependencyProperty.Register(
            "KeyVaultItem", typeof (IKeyVaultItemViewModel), typeof (FlippableItemTile), null);
        public IKeyVaultItemViewModel KeyVaultItem
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Brush _background;
        private IKeyVaultItemViewModel _viewModel;

        public FlippableItemTile()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
