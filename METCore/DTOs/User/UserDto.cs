using static METCore.Enums.Types;

namespace METCore.DTOs.User
{
    public class UserDto(int? Id, string Username, string Password, string FirstName, string LastName, string Email, string Tlf, bool? Active, RoleEnum? Role)
    {
        public int Id { get; set; } = Id ?? 0;

        public string Username { get; set; } = Username;

        public string? Password { get; set; } = Password;

        public string FirstName { get; set; } = FirstName;

        public string LastName { get; set; } = LastName;

        public string Email { get; set; } = Email;

        public string? Tlf { get; set; } = Tlf;

        public bool? Active { get; set; } = Active;

        public RoleEnum? Role { get; set; } = Role;
    }
}
