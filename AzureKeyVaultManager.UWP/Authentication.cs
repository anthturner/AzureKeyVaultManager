using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;

namespace AzureKeyVaultManager.UWP
{
    internal static class Authentication
    {
        private const string GraphApiResource = "https://graph.windows.net/";
        private const string ManagementApiResource = "https://management.core.windows.net/";
        private const string PowershellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

        internal async static Task<WebTokenResponse> GetGraphApiToken()
        {
            return await PerformRequest(GraphApiResource);
        }

        internal async static Task<WebTokenResponse> GetManagementApiToken()
        {
            return await PerformRequest(ManagementApiResource);
        }

        internal async static Task<WebTokenResponse> GetToken(string authority, string resource)
        {
            // todo: is authority needed since we're using a global authority to begin with?
            return await PerformRequest(resource, authority);
        }

        private async static Task<WebTokenResponse> PerformRequest(string resource, string authority = "organizations")
        {
            WebAccountProvider wap = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", authority);
            var tokenRequest = new WebTokenRequest(wap, string.Empty, PowershellClientId);
            tokenRequest.Properties.Add("resource", resource);
            var tokenResponse = await WebAuthenticationCoreManager.RequestTokenAsync(tokenRequest);
            if (tokenResponse.ResponseStatus != WebTokenRequestStatus.Success)
                throw new Exception(tokenResponse.ResponseError.ErrorMessage);
            return tokenResponse.ResponseData.Single();
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