using METCore.DTOs.Shared;
using METCore.DTOs.User;

namespace MobileApp.Services
{
    public class AccountService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory)
    {
        // DIRECT TRANSLATION of your API call from AccountController
        public async Task<bool> LoginAsync(string identifier, string password)
        {
            try
            {
                var loginDto = new LogInDto(identifier, password);

                // Same API call as your WebApp makes
                var response = await SendRequest(HttpMethod.Post, "Auth", "LogIn", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await GetResult<MessageDto>(response);

                    // Store JWT token in secure storage (mobile equivalent of cookies)
                    await SecureStorage.SetAsync("jwt_token", result.Message);
                    await SecureStorage.SetAsync("username", identifier);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void LogoutAsync()
        {
            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("username");
        }

        public static async Task<string?> GetUsernameAsync()
        {
            return await SecureStorage.GetAsync("username");
        }
    }
}