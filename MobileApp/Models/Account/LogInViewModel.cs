using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class LogInViewModel(AccountService authService) : ObservableObject
    {
        private readonly AccountService _authService = authService;

        // DIRECT COPY of properties from LogInViewModel
        [ObservableProperty]
        private string identifier = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        // TRANSLATION of your AccountController LogIn POST logic
        [RelayCommand]
        public async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both identifier and password";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Same login call as your WebApp
                bool success = await _authService.LoginAsync(Identifier, Password);

                if (success)
                {
                    // Navigate to main page (equivalent of RedirectUrl in WebApp)
                    await Shell.Current.GoToAsync("//Views//LogIn");
                }
                else
                {
                    ErrorMessage = "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}