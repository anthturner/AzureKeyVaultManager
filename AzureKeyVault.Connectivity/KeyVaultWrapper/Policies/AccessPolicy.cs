using System.Linq;

namespace AzureKeyVault.Connectivity.KeyVaultWrapper.Policies
{
    public abstract class AccessPolicy
    {
        protected abstract string[] AllPermissions { get; }

        private string[] _accessPermissionString = new string[0];
        public string[] AccessPermissionString
        {
            get
            {
                if (_accessPermissionString.Length == AllPermissions.Length)
                    return new [] {"all"};
                return _accessPermissionString;
            }
            set
            {
                if (value.Length == 1 && value[0] == "all")
                {
                    _accessPermissionString = new string[0];
                    foreach (var permission in AllPermissions)
                        SetOperation(permission, true);
                }
                else
                    _accessPermissionString = value;
            }
        }

        protected bool HasOperation(string operation)
        {
            return _accessPermissionString.Contains(operation);
        }

        protected void SetOperation(string operation, bool state)
        {
            if (HasOperation(operation) && state || !HasOperation(operation) && !state)
                return;

            if (HasOperation(operation) && !state)
                _accessPermissionString = _accessPermissionString.Where(w => w != operation).ToArray();
            else if (!HasOperation(operation) && state)
                _accessPermissionString = _accessPermissionString.Union(new[] { operation }).ToArray();
        }
    }
}
