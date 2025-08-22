using METCore.DTOs.Team;

namespace MobileApp.Services
{
    public class HomeService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory)
    {
        public async Task<IEnumerable<TeamInfoDto>?> GetMyTeamsAsync()
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "MyTeams");
            return response.IsSuccessStatusCode ? await GetResult<IEnumerable<TeamInfoDto>>(response) : null;
        }
    }
}