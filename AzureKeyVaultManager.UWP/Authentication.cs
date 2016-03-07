using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;

namespace AzureKeyVaultManager.UWP
{
    internal static class Authentication
    {
        private const string GraphApiResource = "https://graph.windows.net/";
        private const string ManagementApiResource = "https://management.core.windows.net/";
        private const string PowershellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";
        //private const string AppClientId = "eabceea3-89bc-4b70-903a-8bb46733a96d";
        private const string AppClientId = "af4fabeb-b77f-4680-9dec-efea4f34cd34";

        private const string SelectedClientId = AppClientId;

        internal static string KeyVaultScope = string.Empty;//"email,profile";
        internal static string ActiveDirectoryScope = string.Empty;// "email,profile";

        internal async static Task<WebTokenResponse> GetGraphApiToken()
        {
            return await PerformTokenRequest(GraphApiResource, KeyVaultScope);
        }

        internal async static Task<WebTokenResponse> GetManagementApiToken()
        {
            return await PerformTokenRequest(ManagementApiResource, KeyVaultScope);
        }

        internal async static Task<WebTokenResponse> GetToken(string authority, string scope, string resource)
        {
            // todo: is authority needed since we're using a global authority to begin with?
            return await PerformTokenRequest(resource, scope, authority);
        }

        private async static Task<WebTokenResponse> PerformTokenRequest(string resource, string scope, string authority = "organizations")
        {
            if (resource == "https://vault.azure.net")
            {
                var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority, false, new TokenCache());
                var result = await authContext.AcquireTokenAsync(resource, PowershellClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), PromptBehavior.Auto);
                var accessToken = result.AccessToken;
                return new WebTokenResponse(accessToken);
            }

            WebAccountProvider wap = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", authority);
            var tokenRequest = new WebTokenRequest(wap, string.Empty, SelectedClientId);
            tokenRequest.Properties.Add("resource", resource);
            try
            {
                var tokenResponse = await WebAuthenticationCoreManager.RequestTokenAsync(tokenRequest);
                if (tokenResponse.ResponseStatus != WebTokenRequestStatus.Success)
                    throw new Exception(tokenResponse.ResponseError.ErrorMessage);
                return tokenResponse.ResponseData.Single();
            }
            catch
            {
                throw;
            }
        }
    }

    public static class WebTokenResponseExtensions
    {
        public static string AsBearer(this WebTokenResponse response)
        {
            return $"Bearer {response.Token}";
        }
    }
}