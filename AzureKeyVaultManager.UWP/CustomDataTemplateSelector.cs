using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.UWP.ViewModels;

namespace AzureKeyVaultManager.UWP
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item, null);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IKeyVaultSecret && MainPage.SelectedKeySecret == item)
                return Application.Current.Resources["ExpandedSecretTemplate"] as DataTemplate;
            else if (item is IKeyVaultSecret)
                return Application.Current.Resources["SecretTemplate"] as DataTemplate;

            else if (item is IKeyVaultKey && MainPage.SelectedKeySecret == item)
            {
                //return Application.Current.Resources["ExpandedKeyTemplate"] as DataTemplate;
                var colour = "#D2ABD2";
                var colorObj = Color.FromArgb(0xff, Convert.ToByte(colour.Substring(1, 2), 16), Convert.ToByte(colour.Substring(3, 2), 16), Convert.ToByte(colour.Substring(5, 2), 16));
                
                return new FlippableItemTile() {TileBackground = new SolidColorBrush(colorObj), KeyVaultItem = (IKeyVaultItemViewModel)item};
            }
            else if (item is IKeyVaultKey)
                return Application.Current.Resources["KeyTemplate"] as DataTemplate;
            return null;
        }
    }
}