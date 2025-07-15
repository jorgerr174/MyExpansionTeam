using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Account
{
    public class ProfileViewModel
    {
        public UserViewModel UpdateUser { get; set; } = new UserViewModel();
        public CredentialsViewModel UpdateCredentials { get; set; } = new CredentialsViewModel();
    }

    public class UserViewModel
    {
        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        public string? Tlf { get; set; } = string.Empty;
    }

    public class CredentialsViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? NewUsername { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmNewPassword { get; set; } = string.Empty;
    }
}
