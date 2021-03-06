﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureKeyVault.Connectivity.Rest.Http
{
    class RestClientBase
    {
        private readonly HttpClient _client;

        public RestClientBase(HttpClient client, string version)
        {
            _client = client;
            Version = version;
        }

        protected string Version { get; }

        protected async Task<T> Get<T>(Uri uri)
        {
            var result = await _client.GetAsync(uri);
            return await GetResult<T>(result);
        }

        protected async Task<T> Post<T>(Uri uri, string postData, string mediaType = null)
        {
            HttpResponseMessage result;
            if (mediaType != null)
                result = await _client.PostAsync(uri, new StringContent(postData, System.Text.UTF8Encoding.UTF8, mediaType));
            else
                result = await _client.PostAsync(uri, new StringContent(postData));

            return await GetResult<T>(result);
        }

        protected async Task<dynamic> Post(Uri uri, string postData, string mediaType = null)
        {
            HttpResponseMessage result;
            if (mediaType != null)
                result = await _client.PostAsync(uri, new StringContent(postData, System.Text.UTF8Encoding.UTF8, mediaType));
            else
                result = await _client.PostAsync(uri, new StringContent(postData));
            return await GetResultDynamic(result);
        }

        protected async Task<T> Put<T>(Uri uri, string putData, string mediaType = null)
        {
            HttpResponseMessage result;
            if (mediaType != null)
                result = await _client.PutAsync(uri, new StringContent(putData, System.Text.UTF8Encoding.UTF8, mediaType));
            else
                result = await _client.PutAsync(uri, new StringContent(putData));
            return await GetResult<T>(result);
        }

        protected async Task Delete(Uri uri)
        {
            var result = await _client.DeleteAsync(uri);
            await GetResult(result);
        }

        private async Task GetResult(HttpResponseMessage response)
        {
            await GetResult<object>(response);
        }

        private async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                dynamic errorResult = (dynamic)JsonConvert.DeserializeObject(content);
                if (errorResult != null && errorResult.error.message != null)
                {
                    var errorMessage = (string)errorResult.error.message;
                    throw new Exception(errorMessage, ex);
                }
                throw;
            }

            if (!string.IsNullOrEmpty(content))
                return JsonConvert.DeserializeObject<T>(content);
            else
                return default(T);
        }

        private async Task<dynamic> GetResultDynamic(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                dynamic errorResult = JsonConvert.DeserializeObject(content);
                if (errorResult != null && errorResult.error.message != null)
                    throw new Exception(errorResult.error.message, ex);
                throw;
            }

            if (!string.IsNullOrEmpty(content))
                return JsonConvert.DeserializeObject(content);
            else
                return null;
        }
    }
}
