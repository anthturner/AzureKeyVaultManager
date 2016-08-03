using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;

namespace AzureKeyVaultManager.UWP.ServiceAuthentication
{
    public class AdalAuthentication : Authentication
    {
        private const string LoginBase = "https://login.microsoftonline.com";
        private const string PowershellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

        private static TokenCache TokenCache = new TokenCache();
        private Dictionary<string, AuthenticationContext> Contexts = new Dictionary<string, AuthenticationContext>();

        protected override async Task<WebTokenResponse> GetToken(string resource, string authority = null)
        {
            if (authority == null)
                authority = "common";

            if (!Contexts.ContainsKey($"{LoginBase}/{authority}"))
                Contexts.Add($"{LoginBase}/{authority}", new AuthenticationContext($"{LoginBase}/{authority}", true, TokenCache));

            var authContext = Contexts[$"{LoginBase}/{authority}"];

            try
            {
                var token = authContext.AcquireTokenSilentAsync(resource, PowershellClientId).Result;
                return new WebTokenResponse(token.AccessToken);
            }
            catch (Exception ex) { }

            AuthenticationResult result;
            if (TokenCache.Count == 0)
            {
                await Task.Delay(1000);
                result = await authContext.AcquireTokenAsync(resource, PowershellClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Always, false));
            }
            else
                result = await authContext.AcquireTokenAsync(resource, PowershellClientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto, false));
            var accessToken = result.AccessToken;
            return new WebTokenResponse(accessToken);
        }
    }
}
