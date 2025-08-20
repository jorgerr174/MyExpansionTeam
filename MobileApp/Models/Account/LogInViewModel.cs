using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class LogInViewModel(AccountService authService) : BaseViewModel
    {
        private readonly AccountService _accountService = authService;
        [ObservableProperty] private string identifier = string.Empty;
        [ObservableProperty] private string password = string.Empty;


        [RelayCommand]
        public async Task LogInAsync()
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
                if (await _accountService.LogInAsync(Identifier, Password))
                    await _accountService.GoToHomeTabAsync();
                else
                    ErrorMessage = "Invalid credentials";
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