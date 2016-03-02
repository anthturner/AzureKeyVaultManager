using AzureKeyVaultManager.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            if (MainPage.SelectedKeySecret == item)
            {
                if (item is IKeyVaultSecret)
                    return Application.Current.Resources["flippableSecretCard"] as DataTemplate;
                else
                    return Application.Current.Resources["flippableKeyCard"] as DataTemplate;
            }
            else
                return Application.Current.Resources["smallCard"] as DataTemplate;
        }
    }
}