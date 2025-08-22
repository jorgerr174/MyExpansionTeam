using System.Net.Http.Headers;
using System.Text.Json;
using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebApp.Controllers
{
    [Controller]
    public abstract class BaseController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
    {
        private readonly HttpClient _fastClient = httpClientFactory.CreateClient("_fastClient");
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("_httpClient");
        private readonly HttpClient _importClient = httpClientFactory.CreateClient("_importClient");
        private readonly IConfiguration _configuration = configuration;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };


        #region SendRequest
        protected async Task<HttpResponseMessage> SendRequest(HttpMethod method, string controller, string function, Object? obj = null)
        {
            string url = $"{controller}/{function}";

            HttpRequestMessage request = new(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
            return (obj is FileDto fileDto && fileDto.File is not null) ? await SendImportRequest(request, fileDto) : await SendNormalRequest(request, url, obj);
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

        private async Task<HttpResponseMessage> SendNormalRequest(HttpRequestMessage request, string ogUrl, Object? obj = null)
        {
            if (obj != null)
                if (request.Method == HttpMethod.Get && obj is string[] stringList)
                    request.RequestUri = new Uri(_httpClient.BaseAddress, $"{ogUrl}?{String.Join('&', stringList)}");
                else
                    request.Content = JsonContent.Create(obj);
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
                var successResult = JsonSerializer.Deserialize<ResultDto<T>>(content, _jsonOptions);
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
        #endregion GetResult


        #region RenderViewToString
        protected static async Task<ResultDto<string>> RenderViewToString(Controller controller, string viewNamePath, object model)
        {
            if (string.IsNullOrEmpty(viewNamePath))
                viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using StringWriter writer = new StringWriter();
            try
            {
                IViewEngine? viewEngine = controller.HttpContext.RequestServices.GetService<ICompositeViewEngine>();
                if (viewEngine is null)
                    return new ResultDto<string>("Error during view load.");

                ViewEngineResult viewResult = viewNamePath.EndsWith(".cshtml")
                    ? viewEngine.GetView(viewNamePath, viewNamePath, false)
                    : viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

                if (!viewResult.Success)
                    return new ResultDto<string>(String.Format("A view with the name {0} could not be found", viewNamePath));

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return new ResultDto<string>(null, writer.GetStringBuilder().ToString());
            }
            catch (Exception ex)
            {
                return new ResultDto<string>(String.Format("Failed - {0}", ex.Message));
            }
        }
        #endregion RenderViewToString
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