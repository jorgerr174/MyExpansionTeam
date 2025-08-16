using METCore.DTOs.Admin;

namespace MobileApp.Services
{
    public class AdminService : BaseService
    {
        public AdminService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<ResultImportDto?> ImportDataAsync(ImportDto importDto, byte[] fileContent, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Add file content
                var fileContent_stream = new ByteArrayContent(fileContent);
                fileContent_stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                content.Add(fileContent_stream, "File", fileName);

                // Add form fields
                content.Add(new StringContent(importDto.Type.ToString()), "Type");
                if (importDto.Year.HasValue)
                    content.Add(new StringContent(importDto.Year.Value.ToString()), "Year");
                content.Add(new StringContent(importDto.StatsType.ToString()), "StatsType");

                var request = new HttpRequestMessage(HttpMethod.Post, "Import/Import");
                var token = await SecureStorage.GetAsync("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                request.Content = content;

                var response = await base._importClient.SendAsync(request);
                return response.IsSuccessStatusCode ? await GetResult<ResultImportDto>(response) : null;
            }
            catch { return null; }
        }
    }
}