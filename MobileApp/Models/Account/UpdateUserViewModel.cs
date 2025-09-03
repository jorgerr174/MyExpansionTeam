using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.User;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class UpdateUserViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;

        [ObservableProperty] private string currentFirstName = string.Empty;
        [ObservableProperty] private string currentLastName = string.Empty;
        [ObservableProperty] private string currentEmail = string.Empty;
        [ObservableProperty] private string currentTlf = string.Empty;

        [ObservableProperty] private string newFirstName = string.Empty;
        [ObservableProperty] private string newLastName = string.Empty;
        [ObservableProperty] private string newEmail = string.Empty;
        [ObservableProperty] private string newTlf = string.Empty;


        [RelayCommand]
        public async Task LoadProfile()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                if (await _accountService.GetProfileAsync() is UserDto profile)
                {
                    CurrentFirstName = profile.FirstName;
                    CurrentLastName = profile.LastName;
                    CurrentEmail = profile.Email;
                    CurrentTlf = profile.Tlf ?? string.Empty;
                }
                else
                {
                    ErrorMessage = "Error al cargar la cuenta";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar la cuenta: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task UpdateUser()
        {
            if (string.IsNullOrWhiteSpace(NewFirstName) && string.IsNullOrWhiteSpace(NewLastName) &&
                string.IsNullOrWhiteSpace(NewEmail) && string.IsNullOrWhiteSpace(NewTlf))
                return;

            if (!IsValidEmail(NewEmail))
            {
                ErrorMessage = "Por favor, introduzca una dirección de correo válida";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                string firstName = string.IsNullOrWhiteSpace(NewFirstName) ? CurrentFirstName : NewFirstName;
                string lastName = string.IsNullOrWhiteSpace(NewLastName) ? CurrentLastName : NewLastName;
                string email = string.IsNullOrWhiteSpace(NewEmail) ? CurrentEmail : NewEmail;
                string tlf = string.IsNullOrWhiteSpace(NewTlf) ? CurrentTlf : NewTlf;

                if (await _accountService.UpdateUserAsync(firstName, lastName, email, tlf))
                    await BaseService.GoToProfileTabAsync();
                else
                    ErrorMessage = "Error al actualizar la cuenta";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al actualizar la cuenta: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}