using METCore.DTOs.Team;

namespace MobileApp.Services
{
    public class HomeService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory)
    {
        public async Task IndexAsync(Views.Home.Index view)
        {
            // if (User.Identity == null) return View();
            bool isAuthenticated = await IsAuthenticatedAsync();

            if (!isAuthenticated)
            {
                view.IsNotAuthenticated();
                return;
            }

            var response = await SendRequest(HttpMethod.Get, "Teams", "MyTeams");

            if (!response.IsSuccessStatusCode)
            {
                view.IsNotAuthenticated();
                return;
            }

            var teams = await GetResult<IEnumerable<TeamInfoDto>>(response);

            view.IsNotAuthenticated();
            view.SetTeams(teams);
        }
    }
}