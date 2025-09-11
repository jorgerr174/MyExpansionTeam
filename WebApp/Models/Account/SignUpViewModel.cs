using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Account
{
    public class SignUpViewModel
    {
        [Required]
        [DisplayName("Nombre de usuario")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$",
            ErrorMessage = "El nombre de usuario, de mínimo 8, caracteres debe contener: una letra y un dígito.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_?.,-]).{8,}$",
            ErrorMessage = "La contraseña, de mínimo 8, caracteres debe contener: una minúscula, una mayúscula, un dígito y un símbolo.")]
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
        [RegularExpression(@"^(\d{9})?$", ErrorMessage = "El teléfono debe tener nueve dígitos.")]
        public string? Tlf { get; set; } = string.Empty;
    }
}
