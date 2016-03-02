using System;
using AzureKeyVaultManager.UWP.ViewModels;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Markup;

namespace AzureKeyVaultManager.UWP.Converters
{
    public class ItemTypeToBrushConverter : IValueConverter
    {
        private static Color KeyTileColor = (Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), "#D2ABD2");
        private static Color SecretTileColor = (Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), "#ABD2D2");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is KeyVaultKeyViewModel)
            {
                return new SolidColorBrush(KeyTileColor);
            }
            else if (value is KeyVaultSecretViewModel)
            {
                return new SolidColorBrush(SecretTileColor);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
