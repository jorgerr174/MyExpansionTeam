using System.ComponentModel.DataAnnotations;

namespace METCore.DTOs.Team
{
    public class TeamBasicInfoDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(3)]
        public string Abb { get; set; }

        [Required]
        public string Mascot { get; set; }

        public string UserUsername { get; set; }

        public DateTime Date { get; set; }

        public bool Complete { get; set; }


        public TeamBasicInfoDto()
        {
            this.Id = 0;
            this.Location = string.Empty;
            this.Abb = string.Empty;
            this.Mascot = string.Empty;
            this.UserUsername = string.Empty;
            this.Date = DateTime.Now;
            this.Complete = false;
        }

        public TeamBasicInfoDto(int Id, string Location, string Abb, string Mascot)
        {
            this.Id = Id;
            this.Location = Location;
            this.Abb = Abb;
            this.Mascot = Mascot;
            this.UserUsername = string.Empty;
            this.Date = DateTime.Now;
            this.Complete = false;
        }

        public TeamBasicInfoDto(int Id, string Location, string Abb, string Mascot, string UserUsername, DateTime Date, bool? Complete)
            : this(Id, Location, Abb, Mascot)
        {
            this.UserUsername = UserUsername;
            this.Date = Date;
            this.Complete = Complete ?? false;
        }
    }
}
