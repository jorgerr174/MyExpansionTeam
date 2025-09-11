using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;

namespace WebApp.Models.Account
{
    public class LogInViewModel
    {
        [Required]
        [DisplayName("Identificador")]
        public string Identifier { get; set; }

        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_?.,-]).{8,}$",
            ErrorMessage = "El contraseña, de mínimo 8, caracteres debe contener: una minúscula, una mayúscula, un dígito y un símbolo.")]
        public string Password { get; set; }

        [DataType(DataType.Url)]
        public string RedirectUrl { get; set; }


        public LogInViewModel()
        {
            this.Identifier = string.Empty;
            this.Password = string.Empty;
            this.RedirectUrl = string.Empty;
        }

        public LogInViewModel(string? RedirectUrl = null)
        {
            this.Identifier = string.Empty;
            this.Password = string.Empty;
            this.RedirectUrl = RedirectUrl ?? string.Empty;
        }
    }
}
