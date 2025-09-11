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

        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$",
            ErrorMessage = "El nombre de usuario, de mínimo 8, caracteres debe contener: una letra y un dígito.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_?.,-]).{8,}$",
            ErrorMessage = "La contraseña, de mínimo 8, caracteres debe contener: una minúscula, una mayúscula, un dígito y un símbolo.")]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{9})?$", ErrorMessage = "El teléfono debe tener nueve dígitos.")]
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
        #endregion
    }
}
