using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class SignUpViewModel(AccountService authService) : BaseViewModel
    {
        private readonly AccountService _accountService = authService;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string tlf = string.Empty;

        [RelayCommand]
        public async Task SignUpAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword)
                || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please fill all required fields";
                return;
            }

            if (!Password.Equals(ConfirmPassword))
            {
                ErrorMessage = "The passwords dont match";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                if (await _accountService.SignUpAsync(Username, Password, ConfirmPassword, FirstName, LastName, Email, Tlf))
                    await _accountService.GoToAsync(AppRoutes.LogIn, null);
                else
                    ErrorMessage = "Sign up failed";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Sign up failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand] public async Task GoBack() => await _accountService.GoBackAsync(null);
    }
}