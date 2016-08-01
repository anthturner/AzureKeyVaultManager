namespace AzureKeyVault.Connectivity.KeyVaultWrapper
{
    public enum KeyVaultAlgorithm
    {
        /// <summary>
        /// RSAES-PKCS1-V1_5 [RFC3447] key encryption
        /// </summary>
        RSA1_5,

        /// <summary>
        /// RSAES using Optimal Asymmetric Encryption Padding (OAEP) [RFC3447], with the default parameters specified by RFC 3447 in Section A.2.1. Those default parameters are using a hash function of SHA-1 and a mask generation function of MGF1 with SHA-1
        /// </summary>
        RSA_OAEP,

        /// <summary>
        /// RSASSA-PKCS-v1_5 using SHA-256. The application supplied digest value must be computed using SHA-256 and must be 32 bytes in length
        /// </summary>
        RS256,

        /// <summary>
        /// RSASSA-PKCS-v1_5 using SHA-384. The application supplied digest value must be computed using SHA-384 and must be 48 bytes in length
        /// </summary>
        RS384,

        /// <summary>
        /// RSASSA-PKCS-v1_5 using SHA-512. The application supplied digest value must be computed using SHA-512 and must be 64 bytes in length
        /// </summary>
        RS512,

        /// <summary>
        /// See [RFC2437], a specialized use-case to enable certain TLS scenarios.
        /// </summary>
        RSNULL
    }

    public static class KeyVaultAlgorithmExtensions
    {
        public static string GetConfigurationString(this KeyVaultAlgorithm algorithm)
        {
            // Handle only naming exceptions to the switch, default to ToString()
            switch (algorithm)
            {
                case KeyVaultAlgorithm.RSA_OAEP:
                    return "RSA-OAEP";
                default:
                    return algorithm.ToString();
            }
        }

        public static bool CanCryptOrWrap(this KeyVaultAlgorithm algorithm)
        {
            return algorithm == KeyVaultAlgorithm.RSA1_5 ||
                   algorithm == KeyVaultAlgorithm.RSA_OAEP;
        }

        public static bool CanSignOrVerify(this KeyVaultAlgorithm algorithm)
        {
            return algorithm == KeyVaultAlgorithm.RS256 ||
                   algorithm == KeyVaultAlgorithm.RS384 ||
                   algorithm == KeyVaultAlgorithm.RS512 ||
                   algorithm == KeyVaultAlgorithm.RSNULL;
        }
    }
}
