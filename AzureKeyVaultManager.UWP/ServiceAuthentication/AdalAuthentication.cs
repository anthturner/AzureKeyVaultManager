using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureKeyVaultManager.UWP.ServiceAuthentication
{
    public class AdalAuthentication : Authentication
    {
        private const string LoginBase = "https://login.microsoftonline.com";
        private const string PowershellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

        private static TokenCache TokenCache = new TokenCache();

        protected override async Task<WebTokenResponse> GetToken(string resource, string authority = null)
        {
            if (authority == null)
                authority = "common";
            
            var authContext = new AuthenticationContext($"{LoginBase}/{authority}", true, TokenCache);
            AuthenticationResult result;
            if (TokenCache.Count == 0)
                result = await authContext.AcquireTokenAsync(resource, PowershellClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), PromptBehavior.Always);
            else
                result = await authContext.AcquireTokenAsync(resource, PowershellClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), PromptBehavior.Auto);
            var accessToken = result.AccessToken;
            return new WebTokenResponse(accessToken);
        }
    }
}
