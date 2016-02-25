using AzureKeyVaultManager.Contracts;
using AzureKeyVaultManager.SimulatedTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVaultManager
{
    public class KeyVaultServiceSimulator : IKeyVaultService
    {
        public async Task<ICollection<IKeyVaultSecret>> GetSecrets()
        {
            await Task.Yield();

            return new List<IKeyVaultSecret>()
            {
                new KeyVaultSecret() { Name = "Secret One", Value = "I'm hidden!", Expires = DateTime.Now.AddDays(6), ValidAfter = DateTime.Now.AddDays(1)},
                new KeyVaultSecret() { Name = "Secret Two", Value = "p@ssW0rd1", Expires = DateTime.Now.AddDays(21)}
            };
        }

        public Task<string> GetSecretValue(IKeyVaultSecret secret)
        {
            return Task.FromResult("I'm a secret");
        }

        public async Task<ICollection<IKeyVaultKey>> GetKeys()
        {
            await Task.Yield();

            return new List<IKeyVaultKey>()
            {
                new KeyVaultKey()
                {
                    Name = "Key One",
                    Key = "{ \"kid\":\"https://demo7616.vault.azure.net/keys/mytestkey/e0ec2eb3b8764192aa1e7f20fc74dab7\",\"kty\":\"RSA\",\"key_ops\":[\"encrypt\",\"decrypt\",\"sign\",\"verify\",\"wrapKey\",\"unwrapKey\"],\"n\":\"8jfbzUQDIOo8\",\"e\":\"AQAB\"}",
                }
            };
        }

        public Task<string> GetKeyValue(IKeyVaultKey key)
        {
            throw new NotImplementedException();
        }
    }
}
