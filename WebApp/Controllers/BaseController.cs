using System.Net.Http.Headers;
using System.Text.Json;
using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Controller]
    public abstract class BaseController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
    {
        private readonly HttpClient _fastClient = httpClientFactory.CreateClient("_fastClient");
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("_httpClient");
        private readonly HttpClient _importClient = httpClientFactory.CreateClient("_importClient");
        private readonly IConfiguration _configuration = configuration;
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };


        protected async Task<HttpResponseMessage> SendRequest(HttpMethod method, string controller, string function, Object? obj = null)
        {
            var request = new HttpRequestMessage(method, controller + "/" + function);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
            return (obj is FileDto fileDto && fileDto.File is not null) ? await SendImportRequest(request, fileDto) : await SendNormalRequest(request, obj);
        }


        private async Task<HttpResponseMessage> SendImportRequest(HttpRequestMessage request, FileDto fileDto)
        {
            var streamContent = new StreamContent(fileDto.File.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileDto.File.ContentType);

            var content = new MultipartFormDataContent { { streamContent, "File", fileDto.File.FileName } };
            if (fileDto is ImportDto importDto) content.Add(new StringContent(importDto.Type.ToString()), "Type");

            request.Content = content;
            return await _importClient.SendAsync(request);
        }


        private async Task<HttpResponseMessage> SendNormalRequest(HttpRequestMessage request, Object? obj = null)
        {
            if (obj != null) request.Content = JsonContent.Create(obj);
            return await _httpClient.SendAsync(request);
        }


        protected async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();

            try
            {
                var successResult = JsonSerializer.Deserialize<T>(content, jsonOptions);
                if (successResult != null) return successResult!;
            }
            catch
            {
                // Failed to deserialize into T, try fallback
            }

            var fallback = Activator.CreateInstance<T>();

            var messageProp = typeof(T).GetProperty("Message");
            if (messageProp != null && messageProp.CanWrite)
                messageProp.SetValue(fallback, content);

            return fallback;
        }

        protected async Task<ResultDto<T>> GetResultWithMessage<T>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            try
            {
                var successResult = JsonSerializer.Deserialize<ResultDto<T>>(content, jsonOptions);
                if (successResult != null) return successResult;
            }
            catch
            {
                // Failed to deserialize into ResultDto<T>, try fallback
            }

            // Fallback: create default instance and set Message if possible
            var fallbackValue = Activator.CreateInstance<T>();
            var messageProp = typeof(T).GetProperty("Message");
            if (messageProp != null && messageProp.CanWrite)
                messageProp.SetValue(fallbackValue, content);

            return new ResultDto<T>(content, fallbackValue);
        }
    }

    public static class TempStorage
    {
        public static Dictionary<string, Byte[]> ErrorFiles { get; } = [];

        public static void StoreErrorFile(string fileName, Byte[] fileContent)
        {
            if (ErrorFiles.ContainsKey(fileName)) ErrorFiles[fileName] = fileContent;
            else ErrorFiles.Add(fileName, fileContent);
        }
    }
}