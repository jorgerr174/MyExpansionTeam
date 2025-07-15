namespace METCore.DTOs.User
{
    public class UpdateCredentialsDto(string Password, string NewUsername, string NewPassword)
    {
        public string Password { get; set; } = Password;
        public string? NewUsername { get; set; } = NewUsername;
        public string? NewPassword { get; set; } = NewPassword;
    }
}
