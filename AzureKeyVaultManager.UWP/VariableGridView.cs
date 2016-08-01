using AzureKeyVault.Connectivity.Contracts;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP
{
    public class VariableGridView : GridView
    {
        private const int GridX = 10;
        private const int GridY = 10;

        private const int Margin = 15;

        private static dynamic SecretTile = new 
        {
            Small = new Size(230, 140),
            Large = new Size(350, 290)
        };
        private static dynamic KeyTile = new
        {
            Small = new Size(230, 140),
            Large = new Size(350, 290)
        };

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            Size result;

            if (item is IKeyVaultSecret)
                result = (item == MainPage.SelectedKeySecret) ? SecretTile.Large : SecretTile.Small;
            else if (item is IKeyVaultKey)
                result = (item == MainPage.SelectedKeySecret) ? KeyTile.Large : KeyTile.Small;

            result.Width += 2*Margin;
            result.Height += 2*Margin;

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, result.Width / GridX);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, result.Height / GridY);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
