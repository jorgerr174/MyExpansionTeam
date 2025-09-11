using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class SignUpViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;
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
                ErrorMessage = "Por favor, introduzca todos los campos requeridos.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Por favor, introduzca una dirección de correo válida";
                return;
            }

            if (!Regex.Match(Username, @"^(?=.*[a-zA-Z])(?=.*\d).{8,}$").Success)
            {
                ErrorMessage = "El nombre de usuario, de mínimo 8, caracteres debe contener: una letra y un dígito.";
                return;
            }
            if (!Regex.Match(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_?.,-]).{8,}$").Success)
            {
                ErrorMessage = "La contraseña, de mínimo 8, caracteres debe contener: una minúscula, una mayúscula, un dígito y un símbolo.";
                return;
            }

            if (!Regex.Match(Tlf, @"^(\d{9})?$").Success)
            {
                ErrorMessage = "El teléfono debe tener nueve dígitos.";
                return;
            }

            if (!Password.Equals(ConfirmPassword))
            {
                ErrorMessage = "Las contraseñas no coinciden.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                if (await _accountService.SignUpAsync(Username, Password, ConfirmPassword, FirstName, LastName, Email, Tlf))
                    await BaseService.GoToProfileTabAsync();
                else
                    ErrorMessage = "Creación de cuenta fallida";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Creación de cuenta fallida: {ex.Message}";
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