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
