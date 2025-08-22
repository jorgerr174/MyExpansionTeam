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
        private static string ObjectToQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj) != null)
                .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(obj)?.ToString() ?? "")}");
            return string.Join("&", properties);
        }

        protected async Task<HttpResponseMessage> SendRequest(HttpMethod method, string controller, string function, Object? obj = null)
        {
            string url = $"{controller}/{function}";

            HttpRequestMessage request = new(method, url);

            // Get JWT token from secure storage instead of cookies
            string? token = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //if (obj != null)
            //    request.Content = JsonContent.Create(obj);

            if (obj != null)
                if (method == HttpMethod.Get && obj is string[] stringList)
                    request.RequestUri = new Uri(_httpClient.BaseAddress, $"{url}?{String.Join('&', stringList)}");
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
        public static async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            return !string.IsNullOrEmpty(token);
        }
        #endregion GetResult


        #region GoTo
        private static async Task GoTo(string newRoute, Dictionary<string, object>? parameters) => await Shell.Current.GoToAsync(newRoute, true, parameters ?? []);

        public static async Task GoBackAsync(Dictionary<string, object>? parameters) => await GoTo("..", parameters);

        public static async Task GoToHomeTabAsync() => await GoTo(AppRoutes.HomeTab, null);
        public static async Task GoToMyTeamsTabAsync() => await GoTo(AppRoutes.MyTeamsTab, null);
        public static async Task GoToProfileTabAsync() => await GoTo(AppRoutes.ProfileTab, null);

        public static async Task GoToAsync(string newRoute, Dictionary<string, object>? parameters)
            => await GoTo(AppRoutes.BuildRoute(Shell.Current?.CurrentPage?.Title ?? string.Empty, newRoute), parameters ?? []);
        #endregion GoTo
    }

    public static class AppRoutes
    {
        public const string HomeTab = "HomeTab";
        public const string MyTeamsTab = "MyTeamsTab";
        public const string ProfileTab = "ProfileTab";


        public const string EditProfile = "EditProfile";
        public const string SignUp = "SignUp";
        public const string UpdateCredentials = "UpdateCredentials";
        public const string UpdateUser = "UpdateUser";

        public const string Admin = "Admin";
        public const string AssignRoles = "AssignRoles";
        public const string Import = "Import";

        public const string CreateTeam = "CreateTeam";
        public const string TeamDetails = "TeamDetails";
        public const string Draft = "Draft";
        public const string DraftResults = "DraftResults";
        public const string EditTeam = "EditTeam";
        public const string Formation = "Formation";
        public const string ReviewRoster = "ReviewRoster";
        public const string Roster = "Roster";
        public const string RosterSettings = "RosterSettings";
        public const string Trade = "Trade";
        public const string Trades = "Trades";

        private static readonly HashSet<string> ValidRoutes =
        [
            EditProfile, SignUp, UpdateCredentials, UpdateUser,
            Admin, AssignRoles, Import,

            CreateTeam, TeamDetails, DraftResults, EditTeam, Formation, ReviewRoster, Roster, RosterSettings, Trade, Trades
        ];

        private static string GetParentTab(string route)
        {
            return route switch
            {
                Admin or AssignRoles or Import or
                EditProfile or SignUp or UpdateCredentials or UpdateUser => ProfileTab,

                CreateTeam or TeamDetails or DraftResults or EditTeam or Formation or
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