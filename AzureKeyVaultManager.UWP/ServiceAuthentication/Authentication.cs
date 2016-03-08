using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;

namespace AzureKeyVaultManager.UWP.ServiceAuthentication
{
    public abstract class Authentication
    {
        private const string GraphApiResource = "https://graph.windows.net/";
        private const string ManagementApiResource = "https://management.core.windows.net/";
        private const string KeyVaultApiResource = "https://vault.azure.net";

        private static Authentication _instance;
        public static Authentication Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AdalAuthentication();
                return _instance;
            }
        }

        protected Authentication() { }

        public async Task<WebTokenResponse> GetGraphApiToken()
        {
            return await GetToken(GraphApiResource);
        }

        public async Task<WebTokenResponse> GetManagementApiToken()
        {
            return await GetToken(ManagementApiResource);
        }

        public async Task<WebTokenResponse> GetKeyVaultApiToken(string tenant)
        {
            return await GetToken(KeyVaultApiResource, tenant);
        }

        protected abstract Task<WebTokenResponse> GetToken(string resource, string authority = null);
    }

    public static class WebTokenResponseExtensions
    {
        public static string AsBearer(this WebTokenResponse response)
        {
            return $"Bearer {response.Token}";
        }
    }
}