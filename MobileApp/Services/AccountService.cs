using METCore.DTOs.Shared;
using METCore.DTOs.User;

namespace MobileApp.Services
{
    public class AccountService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory)
    {
        #region TryAutoLogIn
        public static async Task<bool> TryAutoLogInAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            return !string.IsNullOrEmpty(token);
        }
        #endregion TryAutoLogIn


        #region GetUsername
        public static async Task<string?> GetUsernameAsync()
        {
            return await SecureStorage.GetAsync("username");
        }
        #endregion GetUsername


        #region GetProfile
        public async Task<UserDto?> GetProfileAsync()
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Users", "Profile");
                return response.IsSuccessStatusCode ? await GetResult<UserDto>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetProfile


        #region CU001 SignUp
        public async Task<bool> SignUpAsync(string username, string password, string confirmPassword, string firstName, string lastName, string email, string tlf)
        {
            try
            {
                var signUpDto = new NewUserDto(username, password, confirmPassword, firstName, lastName, email, tlf);
                var response = await SendRequest(HttpMethod.Post, "Auth", "SignUp", signUpDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { return false; }
        }
        #endregion CU001 SignUp


        #region CU002 LogIn
        public async Task<bool> LogInAsync(string identifier, string password)
        {
            try
            {
                var logInDto = new LogInDto(identifier, password);

                // Same API call as your WebApp makes
                var response = await SendRequest(HttpMethod.Post, "Auth", "LogIn", logInDto);

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
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion CU002 LogIn


        #region CU003 LogOut
        public static void LogOutAsync()
        {
            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("username");
        }
        #endregion CU003 LogOut


        #region CU004 UpdateCredentials
        public async Task<bool> UpdateCredentialsAsync(string currentPassword, string newUsername, string newPassword)
        {
            try
            {
                var updateDto = new UpdateCredentialsDto(currentPassword, newUsername, newPassword);
                var response = await SendRequest(HttpMethod.Put, "Auth", "UpdateCredentials", updateDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion CU004 UpdateCredentials


        #region UpdateUser
        public async Task<bool> UpdateUserAsync(string firstName, string lastName, string email, string tlf)
        {
            try
            {
                return (await SendRequest(HttpMethod.Put, "User", "UpdateUser",
                    new UserDto()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Tlf = tlf
                    })
                    ).IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion UpdateUser


        #region CU005 DeleteUser
        public async Task<bool> DeleteUserAsync()
        {
            try
            {
                var response = await SendRequest(HttpMethod.Delete, "Auth", "DeleteUser");
                if (response.IsSuccessStatusCode)
                {
                    LogOutAsync(); // Clear stored tokens
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        #endregion CU005 DeleteUser
    }
}