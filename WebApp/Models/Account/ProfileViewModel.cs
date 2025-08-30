using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static METCore.Enums.Types;

namespace WebApp.Models.Account
{
    public class ProfileViewModel
    {
        public UserViewModel UpdateUser { get; set; } = new UserViewModel();
        public CredentialsViewModel UpdateCredentials { get; set; } = new CredentialsViewModel();
    }

    public class UserViewModel
    {
        [DisplayName("Nombre")]
        public string? FirstName { get; set; } = string.Empty;

        [DisplayName("Apellidos")]
        public string? LastName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        [DisplayName("Correo")]
        public string? Email { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Teléfono")]
        public string? Tlf { get; set; } = string.Empty;
    }

    public class CredentialsViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña Actual")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Nuevo Nombre de Usuario")]
        public string? NewUsername { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [DisplayName("Nueva Contraseña")]
        public string? NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DisplayName("Repetir Nueva Contraseña")]
        public string? ConfirmNewPassword { get; set; } = string.Empty;
    }
}
