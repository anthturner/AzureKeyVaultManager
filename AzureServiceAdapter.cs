using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Azure;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace AzureKeyVaultManager
{
    public class AzureServiceAdapter
    {
        const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        private static AzureServiceAdapter _instance;
        public static AzureServiceAdapter Instance => _instance ?? (_instance = new AzureServiceAdapter());

        internal static string AADClientId => ConfigurationManager.AppSettings["AADClientId"];
        internal static string KeyVaultClientId => ConfigurationManager.AppSettings["KeyVaultClientId"];
        internal static string KeyVaultClientSecret => ConfigurationManager.AppSettings["KeyVaultClientSecret"];
        internal static string RedirectUri => ConfigurationManager.AppSettings["RedirectUri"];
        internal static string ActiveDirectoryTenantId => ConfigurationManager.AppSettings["ActiveDirectoryTenantId"];
        internal static string WindowsManagementUri => ConfigurationManager.AppSettings["WindowsManagementUri"];
        internal static string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];
        internal static string Subscription => ConfigurationManager.AppSettings["Subscription"];
        internal static string Authority => $"{ActiveDirectoryEndpoint}{ActiveDirectoryTenantId}";
        internal static string GraphUri => "https://graph.windows.net/";
        internal static string GraphAuthority => $"{GraphUri}{ActiveDirectoryTenantId}";

        private FileCache TokenCache { get; set; }

        private AzureServiceAdapter()
        {
            TokenCache = new FileCache();
        }

        public void ClearTokenCache()
        {
            TokenCache.Clear();
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }

        public KeyVaultManagementClient CreateKeyVaultManagementClient()
        {
            return new KeyVaultManagementClient(new TokenCloudCredentials(Subscription, GetAuthenticationResult(Authority, WindowsManagementUri).AccessToken));
        }

        public KeyVaultClient CreateKeyVaultClient()
        {
            return new KeyVaultClient(GetKeyVaultAccessTokenCallback);
        }

        public async Task<List<IDirectoryObject>> GetAllDirectoryObjects(string searchString)
        {
            var objects = new List<IDirectoryObject>();
            var tasks = new List<Task>();
            tasks.Add(SearchServicePrincipals(searchString).ContinueWith(t => objects.AddRange(t.Result)));
            tasks.Add(SearchUsers(searchString).ContinueWith(t => objects.AddRange(t.Result)));
            await Task.WhenAll(tasks);
            return objects;
        }

        private async Task<List<IServicePrincipal>> SearchServicePrincipals(string searchString)
        {
            var adClient = new ActiveDirectoryClient(new Uri(GraphAuthority), () => Task.FromResult(this.GetAuthenticationResult(Authority, GraphUri)?.AccessToken));
            var initialPage = await adClient.ServicePrincipals.Where(p => p.DisplayName.StartsWith(searchString)).ExecuteAsync();
            return await EnumeratePagedCollection(initialPage);
        }

        private async Task<List<IUser>> SearchUsers(string searchString)
        {
            var adClient = new ActiveDirectoryClient(new Uri(GraphAuthority), () => Task.FromResult(this.GetAuthenticationResult(Authority, GraphUri)?.AccessToken));
            var initialPage = await adClient.Users.Where(p => p.DisplayName.StartsWith(searchString)).ExecuteAsync();
            return await EnumeratePagedCollection(initialPage);
        }

        private async Task<List<T>> EnumeratePagedCollection<T>(IPagedCollection<T> collection, List<T> listSoFar = null)
        {
            if (listSoFar == null)
                listSoFar = new List<T>();
            listSoFar.AddRange(collection.CurrentPage);
            if (collection.MorePagesAvailable)
                await EnumeratePagedCollection<T>(await collection.GetNextPageAsync(), listSoFar);
            return listSoFar;
        }

        public async Task<string> GetClientObjectId()
        {
            if (ClientAuthenticatedMode)
                return AzureServiceAdapter.KeyVaultClientId;
            else
            {
                var adClient = new ActiveDirectoryClient(new Uri(GraphAuthority), () => Task.FromResult(this.GetAuthenticationResult(Authority, GraphUri)?.AccessToken));
                var thisUser = await adClient.Me.ExecuteAsync();
                return thisUser.ObjectId;
            }
        }

        public bool ClientAuthenticatedMode => !(string.IsNullOrWhiteSpace(KeyVaultClientId) || string.IsNullOrWhiteSpace(KeyVaultClientSecret));

        public async Task<IDirectoryObject> FetchObjectById(string objectId)
        {
            var adClient = new ActiveDirectoryClient(new Uri(GraphAuthority), () => Task.FromResult(this.GetAuthenticationResult(Authority, GraphUri)?.AccessToken));
            return await adClient.DirectoryObjects.GetByObjectId(objectId).ExecuteAsync();
        }

        public async Task<IEnumerable<ResourceGroup>> GetResourceGroups()
        {
            var groups = new List<ResourceGroup>();
            using (var resourceManagementClient = new ResourceManagementClient(new TokenCredentials(GetAuthenticationResult(Authority, WindowsManagementUri)?.AccessToken)))
            {
                resourceManagementClient.SubscriptionId = Subscription;
                var response = await resourceManagementClient.ResourceGroups.ListAsync();
                groups.AddRange(response);
                while (!string.IsNullOrEmpty(response.NextPageLink))
                {
                    response = await resourceManagementClient.ResourceGroups.ListNextAsync(response.NextPageLink);
                    groups.AddRange(response);
                }
            }
            return groups;
        }

        private AuthenticationResult GetAuthenticationResult(string authority, string resource, bool allowPrompt = true)
        {
            var authContext = new AuthenticationContext(authority, TokenCache);
            AuthenticationResult result = null;
            try
            {
                if (allowPrompt)
                    result = authContext.AcquireToken(resource, AADClientId, new Uri(RedirectUri), PromptBehavior.Auto);
                else
                    result = authContext.AcquireToken(resource, AADClientId, new Uri(RedirectUri), PromptBehavior.Never);

                return result;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "Inner Exception : " + ex.InnerException.Message;
                }
                MessageBox.Show(message);
            }
            return null;
        }

        private async Task<string> GetKeyVaultAccessTokenCallback(string authority, string resource, string scope)
        {
            if (ClientAuthenticatedMode)
            { 
                var clientCredential = new ClientCredential(KeyVaultClientId, KeyVaultClientSecret);
                var context = new AuthenticationContext(authority, null);
                var result = await context.AcquireTokenAsync(resource, clientCredential);
                return result.AccessToken;
            }
            else
                return await Task.Factory.StartNew(() => GetAuthenticationResult(authority, resource, true).AccessToken);
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
    }
}
