namespace AzureKeyVaultManager.KeyVaultWrapper.Policies
{
    public class KeyAccessPolicy : AccessPolicy
    {
        protected const string ENCRYPT_OPERATION = "encrypt";
        protected const string DECRYPT_OPERATION = "decrypt";
        protected const string WRAP_OPERATION = "wrap";
        protected const string UNWRAP_OPERATION = "unwrap";
        protected const string SIGN_OPERATION = "sign";
        protected const string VERIFY_OPERATION = "verify";
        protected const string CREATE_OPERATION = "create";
        protected const string UPDATE_OPERATION = "update";
        protected const string DELETE_OPERATION = "delete";
        protected const string BACKUP_OPERATION = "backup";
        protected const string RESTORE_OPERATION = "restore";
        protected const string IMPORT_OPERATION = "import";

        protected override string[] AllPermissions => new[]
        {
            ENCRYPT_OPERATION,
            DECRYPT_OPERATION,
            WRAP_OPERATION,
            UNWRAP_OPERATION,
            SIGN_OPERATION,
            VERIFY_OPERATION,
            CREATE_OPERATION,
            UPDATE_OPERATION,
            DELETE_OPERATION,
            BACKUP_OPERATION,
            RESTORE_OPERATION,
            IMPORT_OPERATION
        };

        public bool CanEncrypt
        {
            get { return HasOperation(ENCRYPT_OPERATION); }
            set { SetOperation(ENCRYPT_OPERATION, value); }
        }
        public bool CanDecrypt
        {
            get { return HasOperation(DECRYPT_OPERATION); }
            set { SetOperation(DECRYPT_OPERATION, value); }
        }

        public bool CanWrap
        {
            get { return HasOperation(WRAP_OPERATION); }
            set { SetOperation(WRAP_OPERATION, value); }
        }
        public bool CanUnwrap
        {
            get { return HasOperation(UNWRAP_OPERATION); }
            set { SetOperation(UNWRAP_OPERATION, value); }
        }

        public bool CanSign
        {
            get { return HasOperation(SIGN_OPERATION); }
            set { SetOperation(SIGN_OPERATION, value); }
        }
        public bool CanVerify
        {
            get { return HasOperation(VERIFY_OPERATION); }
            set { SetOperation(VERIFY_OPERATION, value); }
        }

        public bool CanCreate
        {
            get { return HasOperation(CREATE_OPERATION); }
            set { SetOperation(CREATE_OPERATION, value); }
        }
        public bool CanUpdate
        {
            get { return HasOperation(UPDATE_OPERATION); }
            set { SetOperation(UPDATE_OPERATION, value); }
        }
        public bool CanDelete
        {
            get { return HasOperation(DELETE_OPERATION); }
            set { SetOperation(DELETE_OPERATION, value); }
        }

        public bool CanBackup
        {
            get { return HasOperation(BACKUP_OPERATION); }
            set { SetOperation(BACKUP_OPERATION, value); }
        }
        public bool CanRestore
        {
            get { return HasOperation(RESTORE_OPERATION); }
            set { SetOperation(RESTORE_OPERATION, value); }
        }
        public bool CanImport
        {
            get { return HasOperation(IMPORT_OPERATION); }
            set { SetOperation(IMPORT_OPERATION, value); }
        }
    }
}
