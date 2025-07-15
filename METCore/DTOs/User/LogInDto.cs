namespace METCore.DTOs.User
{
    public class LogInDto(string Identifier, string Password)
    {
        public string Identifier { get; set; } = Identifier;
        public string Password { get; set; } = Password;
    }
}
