using System;
using AzureKeyVaultManager.UWP.ViewModels;
using Windows.UI.Xaml.Data;

namespace AzureKeyVaultManager.UWP.Converters
{
    public class ItemTypeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // NOTE: XAML implements glyphs as "&#xf084;" but codebehind implements as "\uf084"
            if (value is KeyVaultKeyViewModel)
            {
                return "\uf084";
            }
            else if (value is KeyVaultSecretViewModel)
            {
                return "\uf21b";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
