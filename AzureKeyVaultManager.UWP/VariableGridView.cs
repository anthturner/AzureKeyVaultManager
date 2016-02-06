using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP
{
    public class VariableGridView : GridView
    {
        private const int GridX = 10;
        private const int GridY = 10;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var width = 0;
            var height = 0;
            if (item is IKeyVaultSecret)
            {
                if (item == MainPage.SelectedKeySecret)
                {
                    width = 350+15+15; height = 200+15+15;
                }
                else
                {
                    width = 225+15+15; height = 140+15+15;
                }
            }

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, width/GridX);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, height/GridY);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
