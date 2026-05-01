using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Famnances.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpHelper(
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _navigationManager = navigationManager;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
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
        private HttpClient GetClientForUri(string fullUri)
        {
            if (fullUri.Contains("Auth"))
            {
                return _httpClientFactory.CreateClient("AuthService");
            }
            return _httpClientFactory.CreateClient("FamnancesService");
        }

        private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
            {
#if DEBUG
                var json = JsonSerializer.Serialize(value);
#endif
                request.Content = JsonContent.Create(value);
            }
            return request;
        }

        private async Task addJwtHeader(HttpRequestMessage request)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string? token = httpContext?.Session.GetString("TOKEN");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task sendRequest(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            using var httpClient = GetClientForUri(request.RequestUri.ToString());
            using var response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("Login/logout");
                return;
            }

            await handleErrors(response);
        }

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            using var httpClient = GetClientForUri(request.RequestUri.ToString());
            using var response = await httpClient.SendAsync(request);
#if DEBUG
            string responseCheck = await response.Content.ReadAsStringAsync();
#endif
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.UnsupportedMediaType)
            {
                _navigationManager.NavigateTo("Login/logout");
                return default;
            }

            await handleErrors(response);

            if (typeof(T) == typeof(string) && response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == "text/plain")
            {
                string rawString = await response.Content.ReadAsStringAsync();
                return (T)(object)rawString;
            }

            if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                return default;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


            T? result = await response.Content.ReadFromJsonAsync<T>(options);

            if (result == null && response.Content.Headers.ContentLength > 0)
            {
                throw new InvalidOperationException("La deserialización del JSON falló o devolvió nulo.");
            }

            return result;
        }

        private async Task handleErrors(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
        }
    }
}