using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace AzureKeyVaultManager
{
    internal class AdalHelper
    {
        const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        private static AdalHelper _instance;
        internal static AdalHelper Instance => _instance ?? (_instance = new AdalHelper());

        internal AuthenticationContext AuthContext { get; private set; }

        internal static string AADClientId => ConfigurationManager.AppSettings["AADClientId"];
        internal static string KeyVaultClientId => ConfigurationManager.AppSettings["KeyVaultClientId"];
        internal static string KeyVaultClientSecret => ConfigurationManager.AppSettings["KeyVaultClientSecret"];
        internal static string RedirectUri => ConfigurationManager.AppSettings["RedirectUri"];
        internal static string ActiveDirectoryTenantId => ConfigurationManager.AppSettings["ActiveDirectoryTenantId"];
        internal static string WindowsManagementUri => ConfigurationManager.AppSettings["WindowsManagementUri"];
        internal static string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];
        internal static string Subscription => ConfigurationManager.AppSettings["Subscription"];
        internal static string Authority => $"{ActiveDirectoryEndpoint}{ActiveDirectoryTenantId}";

        private AdalHelper()
        {
            AuthContext = new AuthenticationContext(Authority, new FileCache());
        }

        internal string GetAccessToken(bool promptIfNeeded = true)
        {
            AuthenticationResult result = null;
            try
            {
                result = AuthContext.AcquireToken(WindowsManagementUri, AADClientId, new Uri(RedirectUri), PromptBehavior.Never);
                return result.AccessToken;
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "user_interaction_required")
                {
                    // There are no tokens in the cache.
                    if (promptIfNeeded)
                    {
                        return SignIn();
                    }
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }
                    MessageBox.Show(message);
                }
            }
            return null;
        }

        internal void ClearCache()
        {
            AuthContext.TokenCache.Clear();
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }

        internal async Task<IEnumerable<string>> GetResourceGroups()
        {
            var groups = new List<string>();
            using (var resourceManagementClient = new ResourceManagementClient(new TokenCredentials(GetAccessToken(false))))
            {
                resourceManagementClient.SubscriptionId = Subscription;
                var response = await resourceManagementClient.ResourceGroups.ListAsync();
                groups.AddRange(response.Select(r => r.Name));
                while (!string.IsNullOrEmpty(response.NextPageLink))
                {
                    response = await resourceManagementClient.ResourceGroups.ListNextAsync(response.NextPageLink);
                    groups.AddRange(response.Select(r => r.Name));
                }
            }
            return groups;
        }

        private string SignIn()
        {
            AuthenticationResult result = null;
            try
            {
                result = AuthContext.AcquireToken(WindowsManagementUri, AADClientId, new Uri(RedirectUri), PromptBehavior.Always);
                return result.AccessToken;
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "authentication_canceled")
                {
                    MessageBox.Show("Sign in was canceled by the user");
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }
            }
            return null;
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
    }
}
