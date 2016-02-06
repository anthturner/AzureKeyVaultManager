using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        private const int GridX = 10;
        private const int GridY = 10;

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item, null);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DataTemplate template = null;
            int width = 0, height = 0;
            if (item is IKeyVaultSecret && MainPage.SelectedKeySecret == item)
            {
                template = App.Current.Resources["ExpandedSecretTemplate"] as DataTemplate;
                width = 350;
            }
            else if (item is IKeyVaultSecret)
            {
                template = App.Current.Resources["SecretTemplate"] as DataTemplate;
                width = 230;
                height = 140;
            }
            return template;
        }
    }
}