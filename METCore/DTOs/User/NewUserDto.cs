namespace METCore.DTOs.User
{
    public class NewUserDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string? Tlf { get; set; }


        public NewUserDto()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.Email = string.Empty;
        }

        public NewUserDto(string Username, string Password, string ConfirmPassword, string FirstName, string LastName, string Email, string Tlf)
        {
            this.Username = Username;
            this.Password = Password;
            this.ConfirmPassword = ConfirmPassword;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Tlf = Tlf;
        }
    }
}