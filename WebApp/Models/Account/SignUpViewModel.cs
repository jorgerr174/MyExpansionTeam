using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Account
{
    public class SignUpViewModel
    {
        [Required]
        [DisplayName("Nombre de usuario")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Contraseñas no coinciden.")]
        [DisplayName("Repetir Contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [DisplayName("Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Apellidos")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [DisplayName("Teléfono")]
        public string? Tlf { get; set; } = string.Empty;
    }
}
