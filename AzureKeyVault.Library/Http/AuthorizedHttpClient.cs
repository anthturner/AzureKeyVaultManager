using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.Http
{
    public class AuthorizedHttpClient : HttpClient
    {
        private readonly string _bearerToken;

        public AuthorizedHttpClient(string token)
        {
            _bearerToken = token;
            this.DefaultRequestHeaders.Add("Authorization", token);
        }
    }
}
