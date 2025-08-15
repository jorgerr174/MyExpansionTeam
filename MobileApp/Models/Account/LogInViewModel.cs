using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class LogInViewModel : BaseViewModel
    {
        private readonly AccountService _authService;

        public LogInViewModel(AccountService authService)
        {
            _authService = authService;
        }

        [ObservableProperty] private string identifier = string.Empty;
        [ObservableProperty] private string password = string.Empty;


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
                bool success = await _authService.LogInAsync(Identifier, Password);

                if (success)
                {
                    // Navigate to main page (equivalent of RedirectUrl in WebApp)
                    await Shell.Current.GoToAsync("Home/Index");
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