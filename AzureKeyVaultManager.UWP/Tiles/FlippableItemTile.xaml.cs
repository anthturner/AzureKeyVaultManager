using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace AzureKeyVaultManager.UWP.Tiles
{
    public sealed partial class FlippableItemTile : ContentControl
    {
        public static readonly DependencyProperty FrontChildrenProperty = DependencyProperty.Register(
            "FrontChildren",
            typeof(UIElementCollection),
            typeof(FlippableItemTile),
            null);

        public static readonly DependencyProperty BackChildrenProperty = DependencyProperty.Register(
            "BackChildren",
            typeof(UIElementCollection),
            typeof(FlippableItemTile),
            null);

        private bool _cardIsFlipped = false;

        public UIElementCollection FrontChildren
        {
            get { return (UIElementCollection)GetValue(FrontChildrenProperty); }
            private set { SetValue(FrontChildrenProperty, value); }
        }

        public UIElementCollection BackChildren
        {
            get { return (UIElementCollection)GetValue(BackChildrenProperty); }
            private set { SetValue(BackChildrenProperty, value); }
        }

        public FlippableItemTile()
        {
            this.InitializeComponent();
            FrontChildren = displaySection.Children;
            BackChildren = displaySectionBack.Children;
            DataContextChanged += FlippableItemTile_DataContextChanged;
        }

        private void FlippableItemTile_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var cxt = args.NewValue;
        }

        private void Interact_Card(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_cardIsFlipped)
            {
                FlipToFront.Begin();
            }
            else
            {
                FlipToBack.Begin();
            }
            _cardIsFlipped = !_cardIsFlipped;
        }
    }
}
