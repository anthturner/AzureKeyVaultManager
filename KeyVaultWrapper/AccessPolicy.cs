using System;
using System.Linq;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class AccessPolicy
    {
        public bool CanEncrypt
        {
            get { return HasOperation(KeyOperations.Encrypt); }
            set { SetOperation(KeyOperations.Encrypt, value); }
        }
        public bool CanDecrypt
        {
            get { return HasOperation(KeyOperations.Decrypt); }
            set { SetOperation(KeyOperations.Decrypt, value); }
        }

        public bool CanWrap
        {
            get { return HasOperation(KeyOperations.Wrap); }
            set { SetOperation(KeyOperations.Wrap, value); }
        }
        public bool CanUnwrap
        {
            get { return HasOperation(KeyOperations.Unwrap); }
            set { SetOperation(KeyOperations.Unwrap, value); }
        }

        public bool CanSign
        {
            get { return HasOperation(KeyOperations.Sign); }
            set { SetOperation(KeyOperations.Sign, value); }
        }
        public bool CanVerify
        {
            get { return HasOperation(KeyOperations.Verify); }
            set { SetOperation(KeyOperations.Verify, value); }
        }

        public bool CanCreate
        {
            get { return HasOperation(KeyOperations.Create); }
            set { SetOperation(KeyOperations.Create, value); }
        }
        public bool CanUpdate
        {
            get { return HasOperation(KeyOperations.Update); }
            set { SetOperation(KeyOperations.Update, value); }
        }
        public bool CanDelete
        {
            get { return HasOperation(KeyOperations.Delete); }
            set { SetOperation(KeyOperations.Delete, value); }
        }

        public bool CanBackup
        {
            get { return HasOperation(KeyOperations.Backup); }
            set { SetOperation(KeyOperations.Backup, value); }
        }
        public bool CanRestore
        {
            get { return HasOperation(KeyOperations.Restore); }
            set { SetOperation(KeyOperations.Restore, value); }
        }
        public bool CanImport
        {
            get { return HasOperation(KeyOperations.Import); }
            set { SetOperation(KeyOperations.Import, value); }
        }

        public bool CanGet
        {
            get { return HasOperation(KeyOperations.Get); }
            set { SetOperation(KeyOperations.Get, value); }
        }
        public bool CanList
        {
            get { return HasOperation(KeyOperations.List); }
            set { SetOperation(KeyOperations.List, value); }
        }
        public bool CanSet
        {
            get { return HasOperation(KeyOperations.Set); }
            set { SetOperation(KeyOperations.Set, value); }
        }

        private string[] _accessPermissionString = new string[0];
        public string[] AccessPermissionString
        {
            get
            {
                if (_accessPermissionString.Length == Enum.GetNames(typeof (KeyOperations)).Length)
                    return new [] {"all"};
                return _accessPermissionString;
            }
            set
            {
                if (value.Length == 1 && value[0] == "all")
                {
                    _accessPermissionString = new string[0];
                    foreach (var enumName in Enum.GetNames(typeof (KeyOperations)))
                        SetOperation((KeyOperations) Enum.Parse(typeof (KeyOperations), enumName), true);
                }
                else
                    _accessPermissionString = value;
            }
        }

        private bool HasOperation(KeyOperations operation)
        {
            return _accessPermissionString.Contains(GetOperationString(operation));
        }

        private void SetOperation(KeyOperations operation, bool state)
        {
            if (HasOperation(operation) && state || !HasOperation(operation) && !state)
                return;

            if (HasOperation(operation) && !state)
                _accessPermissionString = _accessPermissionString.Where(w => w != GetOperationString(operation)).ToArray();
            else if (!HasOperation(operation) && state)
                _accessPermissionString = _accessPermissionString.Union(new[] { GetOperationString(operation) }).ToArray();
        }

        private string GetOperationString(KeyOperations operation)
        {
            return Enum.GetName(typeof(KeyOperations), operation).ToLower();
        }
    }
}
