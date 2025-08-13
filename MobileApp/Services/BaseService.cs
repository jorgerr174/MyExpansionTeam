using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using METCore.DTOs.Shared;

namespace MobileApp.Services
{
    public class BaseService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _fastClient = httpClientFactory.CreateClient("_fastClient");
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("_httpClient");
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };


        #region SendRequest
        protected async Task<HttpResponseMessage> SendRequest(HttpMethod method, string controller, string function, object? obj = null)
        {
            var request = new HttpRequestMessage(method, $"{controller}/{function}");

            // Get JWT token from secure storage instead of cookies
            var token = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await SendNormalRequest(request, obj);
        }

        private async Task<HttpResponseMessage> SendNormalRequest(HttpRequestMessage request, object? obj = null)
        {
            if (obj != null) request.Content = JsonContent.Create(obj);
            return await _httpClient.SendAsync(request);
        }
        #endregion SendRequest


        #region GetResult
        protected async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();

            try
            {
                var successResult = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                if (successResult != null)
                    return successResult;

                throw new Exception("Deserialization failed");
            }
            catch
            {
                var errorResult = JsonSerializer.Deserialize<MessageDto>(content, _jsonOptions);
                throw new Exception(errorResult?.Message ?? "Unknown error");
            }
        }
        #endregion GetResult


        #region GetResult
        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            return !string.IsNullOrEmpty(token);
        }
        #endregion GetResult
    }
}