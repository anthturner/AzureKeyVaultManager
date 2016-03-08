using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;

namespace AzureKeyVaultManager.UWP.ServiceAuthentication
{
    public class UwpBrokerAuthentication : Authentication
    {
        private const string AppClientId = "af4fabeb-b77f-4680-9dec-efea4f34cd34";
        private const string Authority = "organizations";

        protected override async Task<WebTokenResponse> GetToken(string resource, string authority = null)
        {
            if (authority == null)
                authority = Authority;

            WebAccountProvider wap = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", authority);
            var tokenRequest = new WebTokenRequest(wap, string.Empty, AppClientId);
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
}
