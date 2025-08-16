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
                var userDto = await _accountService.GetProfileAsync();
                if (userDto != null)
                {
                    FirstName = userDto.FirstName;
                    LastName = userDto.LastName;
                    Email = userDto.Email;
                    Tlf = userDto.Tlf;
                    Username = userDto.Username;
                    IsAdmin = userDto.Role == METCore.Enums.Types.RoleEnum.Admin; // Add this line
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
        public async Task GoToAdmin()
        {
            await Shell.Current.GoToAsync("Admin");
        }

        [RelayCommand]
        public async Task EditProfile()
        {
            await Shell.Current.GoToAsync("EditProfile");
        }

        [RelayCommand]
        public async Task LogOut()
        {
            AccountService.LogOutAsync();
            await Shell.Current.GoToAsync("LogIn");
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
                    await Shell.Current.GoToAsync("LogIn");
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