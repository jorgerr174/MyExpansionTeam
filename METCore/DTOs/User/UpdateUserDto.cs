namespace METCore.DTOs.User
{
    public class UpdateUserDto(string? FirstName, string? LastName, string? Email, string? Tlf)
    {
        public string? FirstName { get; set; } = FirstName;

        public string? LastName { get; set; } = LastName;

        public string? Email { get; set; } = Email;

        public string? Tlf { get; set; } = Tlf;
    }
}
