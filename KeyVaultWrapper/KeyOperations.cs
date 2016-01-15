using System;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    [Flags]
    public enum KeyOperations
    {
        Decrypt = 1,
        Encrypt = 2,
        Sign = 4,
        Unwrap = 8,
        Verify = 16,
        Wrap = 32,

        Create = 64,
        Update = 128,
        Delete = 256,

        Backup = 512,
        Restore = 1024,
        Import = 2048,

        Get = 4096,
        List = 8192,
        Set = 16384
    }
}
