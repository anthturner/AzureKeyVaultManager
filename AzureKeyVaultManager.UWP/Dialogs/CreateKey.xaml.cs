using AzureKeyVault.Connectivity.Contracts;
using AzureKeyVault.Connectivity.KeyVaultWrapper;
using AzureKeyVaultManager.UWP.ServiceAuthentication;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AzureKeyVaultManager.UWP.Dialogs
{
    public sealed partial class CreateKey : ContentDialog
    {
        public CreateKey(IKeyVault vault)
        {
            
        }
    }
}