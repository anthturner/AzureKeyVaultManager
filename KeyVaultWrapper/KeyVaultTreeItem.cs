using System.Collections.ObjectModel;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class KeyVaultTreeItem
    {
        public virtual string Header => "Account";
        public virtual ObservableCollection<KeyVaultTreeItem> Children => new ObservableCollection<KeyVaultTreeItem>();
    }
}
