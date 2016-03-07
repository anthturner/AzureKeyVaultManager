using AzureKeyVaultManager.KeyVaultWrapper.Policies;

namespace AzureKeyVaultManager.Contracts
{
    public interface IAccessPolicies
    {
        AccessPolicy KeyPolicy { get; }
        AccessPolicy SecretPolicy { get; }
    }
}
