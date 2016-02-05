using System;
using System.Threading.Tasks;
using AzureKeyVaultManager.KeyVaultWrapper.Policies;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.Management.KeyVault;

namespace AzureKeyVaultManager.KeyVaultWrapper
{
    public class EntityAccessPolicy
    {
        public AccessPolicy KeyPolicy { get; private set; }

        public AccessPolicy SecretPolicy { get; private set; }

        public string ObjectId => OriginalPolicyEntry.ObjectId.ToString("N");

        public string ObjectName { get; set; }

        public string ObjectType { get; set; }

        public AccessPolicyEntry OriginalPolicyEntry { get; private set; }

        public EntityAccessPolicy(AccessPolicyEntry originalPolicyEntry)
        {
            OriginalPolicyEntry = originalPolicyEntry;
            KeyPolicy = new KeyAccessPolicy() {AccessPermissionString = originalPolicyEntry.PermissionsToKeys};
            SecretPolicy = new SecretAccessPolicy() {AccessPermissionString = originalPolicyEntry.PermissionsToSecrets};
        }

        public async Task PopulateDirectoryInformation()
        {
            var fetchedObj = await AzureServiceAdapter.Instance.FetchObjectById(ObjectId);
            ObjectType = fetchedObj?.ObjectType;

            if (fetchedObj is User)
                ObjectName = ((User) fetchedObj).DisplayName;
            else if (fetchedObj is ServicePrincipal)
                ObjectName = ((ServicePrincipal)fetchedObj).DisplayName;
            else if (fetchedObj is Group)
                ObjectName = ((Group)fetchedObj).DisplayName;
            else if (fetchedObj is Application)
                ObjectName = ((Application)fetchedObj).DisplayName;
        }

        public void Update()
        {
            OriginalPolicyEntry.PermissionsToKeys = KeyPolicy.AccessPermissionString;
            OriginalPolicyEntry.PermissionsToSecrets = SecretPolicy.AccessPermissionString;
        }
    }
}
