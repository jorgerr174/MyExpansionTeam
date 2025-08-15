using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly AccountService _accountService;
        public ProfileViewModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string tlf = string.Empty;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private bool isAdmin = false;

        [RelayCommand]
        public async Task LoadProfile()
        {
            IsLoading = true;
            try
            {
                var userInfo = await _accountService.GetProfileAsync();
                if (userInfo != null)
                {
                    FirstName = userInfo.FirstName;
                    LastName = userInfo.LastName;
                    Email = userInfo.Email;
                    Tlf = userInfo.Tlf;
                    Username = await AccountService.GetUsernameAsync() ?? "";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load profile: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task EditProfile()
        {
            await Shell.Current.GoToAsync("Account/EditProfile");
        }

        [RelayCommand]
        public async Task LogOut()
        {
            AccountService.LogOutAsync();
            await Shell.Current.GoToAsync("Account/LogIn");
        }

        [RelayCommand]
        public async Task DeleteUser()
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete",
                "¿Está seguro de que desea eliminar su cuenta?",
                "Yes", "No");

            if (!confirm) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                bool success = await _accountService.DeleteUserAsync();
                if (success)
                {
                    await Shell.Current.GoToAsync("//Login");
                }
                else
                {
                    ErrorMessage = "Failed to delete account";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Delete failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}