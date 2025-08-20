using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class UpdateCredentialsViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;

        [ObservableProperty] private string currentPassword = string.Empty;
        [ObservableProperty] private string newUsername = string.Empty;
        [ObservableProperty] private string newPassword = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;


        [RelayCommand] public static async Task GoBack() => await BaseService.GoBackAsync(null);


        [RelayCommand]
        public async Task UpdateCredentials()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Current password is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUsername) && string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "Please enter new username or new password";
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New passwords don't match";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                if (await _accountService.UpdateCredentialsAsync(CurrentPassword, NewUsername, NewPassword))
                {
                    AccountService.LogOutAsync();
                    await BaseService.GoToAsync(AppRoutes.LogIn, null);
                }
                else
                    ErrorMessage = "Failed to update credentials";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Update failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}