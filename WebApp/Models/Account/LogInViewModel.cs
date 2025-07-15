using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Account
{
    public class LogInViewModel
    {
        [Required]
        public string Identifier { get; set; }

        [Required]
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
