using static METCore.Enums.Types;

namespace METCore.DTOs.Admin
{
    public class AssignRoleDto
    {
        public string Username { get; set; }
        public RoleEnum Role { get; set; }


        public AssignRoleDto() : base()
        {
            this.Username = string.Empty;

        }

        public AssignRoleDto(string Username, RoleEnum Role)
        {
            this.Username = Username;
            this.Role = Role;
        }
    }
}
