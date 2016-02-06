using System;
using System.Windows.Input;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP.Commands
{
    public class ShowSecretCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var obj = (IKeyVaultSecret)parameter;
            // update
        }

        public event EventHandler CanExecuteChanged;
    }
}