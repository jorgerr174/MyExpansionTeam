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


        [RelayCommand]
        public async Task UpdateCredentials()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Contraseña actual es obligatoria.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUsername) && string.IsNullOrWhiteSpace(NewPassword))
                return;

            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Las nuevas contraseñas no coinciden.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                string result = await _accountService.UpdateCredentialsAsync(CurrentPassword, NewUsername, NewPassword);
                if (String.IsNullOrWhiteSpace(result))
                {
                    AccountService.LogOutAsync();
                    await BaseService.GoToProfileTabAsync();
                }
                else
                    ErrorMessage = "Error al actualizar las credenciales: " + result;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Actualización fallida: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}