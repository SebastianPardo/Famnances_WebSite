using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OhMyMoney.Business.Interfaces;

namespace OhMyMoney.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private NavigationManager NavigationManager;
        private string Token;
        public HttpHelper( NavigationManager navigationManager, IHttpContextAccessor httpContext  )
        {
            NavigationManager = navigationManager;
            Token = httpContext.HttpContext.Session.GetString("token");
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            await sendRequest(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            await sendRequest(request);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            await sendRequest(request);
        }

        public async Task<T> Delete<T>(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await sendRequest<T>(request);
        }

        // helper methods

        private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task sendRequest(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            // send request
            HttpClient httpClient = new HttpClient();
            using var response = await httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                NavigationManager.NavigateTo("Login/logout");
                return;
            }

            await handleErrors(response);
        }

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {
            try
            {
                await addJwtHeader(request);

                // send request
                HttpClient httpClient = new HttpClient();
                using var response = await httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();
                // auto logout on 401 response
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.UnsupportedMediaType)
                {
                    NavigationManager.NavigateTo("Login/logout");
                    return default;
                }

                await handleErrors(response);

                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                return await response.Content.ReadFromJsonAsync<T>(options);
            }
            catch (Exception e)
            {
                return default;
            }

        }

        private async Task addJwtHeader(HttpRequestMessage request)
        {            
            //var isApiUrl = !request.RequestUri.IsAbsoluteUri;
            if (Token != null/* && isApiUrl*/)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        private async Task handleErrors(HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
        }
    }
}