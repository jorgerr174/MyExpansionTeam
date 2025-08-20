using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.User;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class ProfileViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;
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
                if (await _accountService.GetProfileAsync() is UserDto userDto)
                {
                    FirstName = userDto.FirstName;
                    LastName = userDto.LastName;
                    Email = userDto.Email;
                    Tlf = userDto.Tlf ?? string.Empty;
                    Username = userDto.Username;
                    IsAdmin = userDto.Role == METCore.Enums.Types.RoleEnum.Admin;
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


        private async Task GoToLogIn() => await _accountService.GoToAsync(AppRoutes.LogIn, null);
        [RelayCommand] public async Task GoToAdmin() => await _accountService.GoToAsync(AppRoutes.Admin, null);
        [RelayCommand] public async Task GoToEditProfile() => await _accountService.GoToAsync(AppRoutes.EditProfile, null);
        [RelayCommand]
        public async Task LogOut()
        {
            AccountService.LogOutAsync();
            await GoToLogIn();
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
                if (await _accountService.DeleteUserAsync()) await GoToLogIn();
                else ErrorMessage = "Failed to delete account";
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