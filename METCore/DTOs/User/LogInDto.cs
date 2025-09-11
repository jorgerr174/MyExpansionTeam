using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace METCore.DTOs.User
{
    public class LogInDto(string Identifier, string Password)
    {
        [Required]
        [DisplayName("Identificador")]
        public string Identifier { get; set; } = Identifier;

        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_?.,-]).{8,}$",
            ErrorMessage = "El contraseña, de mínimo 8, caracteres debe contener: una minúscula, una mayúscula, un dígito y un símbolo.")]
        public string Password { get; set; } = Password;
    }
}
