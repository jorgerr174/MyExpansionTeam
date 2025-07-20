using static METCore.Enums.Types;

namespace METCore.DTOs.User
{
    public class UserDto : NewUserDto
    {
        public int Id { get; set; }

        public bool? Active { get; set; }

        public RoleEnum? Role { get; set; }


        public UserDto() : base()
        {
            this.Id = 0;
        }

        public UserDto(string Username, string Password, string FirstName, string LastName, string Email, string Tlf, int? Id, bool? Active, RoleEnum? Role)
            : base(Username, Password, string.Empty, FirstName, LastName, Email, Tlf)
        {
            this.Id = Id ?? 0;
            this.Active = Active;
            this.Role = Role;
        }
    }
}
