namespace AzureKeyVault.Connectivity.KeyVaultWrapper.Policies
{
    public class SecretAccessPolicy : AccessPolicy
    {
        protected const string GET_OPERATION = "get";
        protected const string LIST_OPERATION = "list";
        protected const string SET_OPERATION = "set";
        protected const string DELETE_OPERATION = "delete";

        protected override string[] AllPermissions => new[]
        {
            GET_OPERATION,
            LIST_OPERATION,
            SET_OPERATION,
            DELETE_OPERATION
        };

        public bool CanGet
        {
            get { return HasOperation(GET_OPERATION); }
            set { SetOperation(GET_OPERATION, value); }
        }
        public bool CanList
        {
            get { return HasOperation(LIST_OPERATION); }
            set { SetOperation(LIST_OPERATION, value); }
        }
        public bool CanSet
        {
            get { return HasOperation(SET_OPERATION); }
            set { SetOperation(SET_OPERATION, value); }
        }
        public bool CanDelete
        {
            get { return HasOperation(DELETE_OPERATION); }
            set { SetOperation(DELETE_OPERATION, value); }
        }
    }
}
