using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class SignUpViewModel : BaseViewModel
    {
        private readonly AccountService _authService;

        public SignUpViewModel(AccountService authService)
        {
            _authService = authService;
        }

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
                bool success = await _authService.SignUpAsync(Username, Password, ConfirmPassword, FirstName, LastName, Email, Tlf);

                if (success)
                    await Shell.Current.GoToAsync("LogIn");
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
    }
}