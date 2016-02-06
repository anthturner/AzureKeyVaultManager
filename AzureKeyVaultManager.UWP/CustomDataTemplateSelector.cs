using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is IKeyVault)
                return App.Current.Resources["VaultTemplate"] as DataTemplate;
            else if (item is IKeyVaultSecret)
                return App.Current.Resources["SecretTemplate"] as DataTemplate;
            return null;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}