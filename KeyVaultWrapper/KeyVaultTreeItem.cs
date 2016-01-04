using System.Collections.ObjectModel;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public abstract class KeyVaultTreeItem
    {
        public abstract string Header { get; }
        public virtual ObservableCollection<KeyVaultTreeItem> Children => new ObservableCollection<KeyVaultTreeItem>();
    }
}
