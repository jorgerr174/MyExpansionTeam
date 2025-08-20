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
        protected readonly HttpClient _importClient = httpClientFactory.CreateClient("_importClient");
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


        #region GoTo
        public async Task GoBackAsync(Dictionary<string, object>? parameters) => await Shell.Current.GoToAsync("..", true, parameters);

        public async Task GoToHomeTabAsync() => await Shell.Current.GoToAsync(AppRoutes.HomeTab);
        public async Task GoToTeamsTabAsync() => await Shell.Current.GoToAsync(AppRoutes.TeamsTab);
        public async Task GoToMyTeamsTabAsync() => await Shell.Current.GoToAsync(AppRoutes.MyTeamsTab);
        public async Task GoToProfileTabAsync() => await Shell.Current.GoToAsync(AppRoutes.ProfileTab);

        public async Task GoToAsync(string newRoute, Dictionary<string, object>? parameters)
            => await Shell.Current.GoToAsync(AppRoutes.BuildRoute(Shell.Current?.GetType().Name ?? string.Empty, newRoute), true, parameters ?? []);
        #endregion GoTo
    }

    public static class AppRoutes
    {
        // Tab routes
        public const string HomeTab = "HomeTab";
        public const string TeamsTab = "TeamsTab";
        public const string MyTeamsTab = "MyTeamsTab";
        public const string ProfileTab = "ProfileTab";

        // Account routes
        public const string EditProfile = "EditProfile";
        public const string LogIn = "LogIn";
        public const string SignUp = "SignUp";
        public const string UpdateCredentials = "UpdateCredentials";
        public const string UpdateUser = "UpdateUser";

        // Admin routes  
        public const string Admin = "Admin";
        public const string AssignRoles = "AssignRoles";
        public const string Import = "Import";

        // Team routes
        public const string CreateTeam = "CreateTeam";
        public const string TeamDetails = "TeamDetails";
        public const string DraftResults = "DraftResults";
        public const string TeamEdit = "EditTeam";
        public const string Formation = "Formation";
        public const string ReviewRoster = "ReviewRoster";
        public const string Roster = "Roster";
        public const string RosterSettings = "RosterSettings";
        public const string Trade = "Trade";
        public const string Trades = "Trades";

        // All valid routes for validation
        private static readonly HashSet<string> ValidRoutes =
        [
            EditProfile, LogIn, SignUp, UpdateCredentials, UpdateUser,
            Admin, AssignRoles, Import,
            CreateTeam, TeamDetails, DraftResults, TeamEdit, Formation,
            ReviewRoster, Roster, RosterSettings, Trade, Trades
        ];

        private static string GetParentTab(string route)
        {
            return route switch
            {
                Admin or AssignRoles or Import or
                EditProfile or LogIn or SignUp or UpdateCredentials or UpdateUser => ProfileTab,

                CreateTeam or TeamDetails or DraftResults or TeamEdit or Formation or
                ReviewRoster or Roster or RosterSettings or Trade or Trades => MyTeamsTab,

                _ => string.Empty
            };
        }

        public static string BuildRoute(string currentRoute, string newRoute)
        {
            if (!ValidRoutes.Contains(newRoute)) throw new ArgumentException($"Invalid route: {newRoute}");

            string newParent = GetParentTab(newRoute);
            return GetParentTab(currentRoute) != newParent ? $"//{newParent}/{newRoute}" : newRoute;
        }
    }
}