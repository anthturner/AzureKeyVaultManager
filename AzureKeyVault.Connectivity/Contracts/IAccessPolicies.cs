using AzureKeyVault.Connectivity.KeyVaultWrapper.Policies;

namespace AzureKeyVault.Connectivity.Contracts
{
    public interface IAccessPolicies
    {
        AccessPolicy KeyPolicy { get; }
        AccessPolicy SecretPolicy { get; }
    }
}
