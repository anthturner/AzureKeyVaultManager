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
        Wrap = 32
    }
}
