using System.ComponentModel.DataAnnotations;
using static METCore.Enums.Types;

namespace METCore.Models
{
    public class User : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Tlf { get; set; }

        public bool Active { get; set; }

        public RoleEnum Role { get; set; }

        #region Not Mapped
        #endregion

        #endregion


        #region Constructors
        public User()
        {
            this.FirstName = "";
            this.LastName = "";
            this.Username = "";
            this.Password = "";
            this.Email = "";
            this.Active = true;
            this.Role = RoleEnum.User;
        }

        public User(string firstName, string lastName, string username, string password, string email, string? tlf, bool? active)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.Tlf = tlf;
            this.Active = active ?? true;
            this.Role = RoleEnum.User;
        }
        #endregion
    }
}
