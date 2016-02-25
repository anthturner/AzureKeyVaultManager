using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;

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
                return Application.Current.Resources["ExpandedKeyTemplate"] as DataTemplate;
            else if (item is IKeyVaultKey)
                return Application.Current.Resources["KeyTemplate"] as DataTemplate;
            return null;
        }
    }
}