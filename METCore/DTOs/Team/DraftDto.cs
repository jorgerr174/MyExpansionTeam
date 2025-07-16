using System.ComponentModel.DataAnnotations;

namespace METCore.DTOs.Team
{
    public class DraftDto : TeamBasicInfoDto
    {
        #region Attributes
        [Required]
        [Range(0, 7)]
        public int Rounds { get; set; }

        [Required]
        [Range(0, 1)]
        public decimal Speed { get; set; }

        [Required]
        public string? Draft { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public DraftDto() : base()
        {
            this.Rounds = 3;
            this.Speed = (decimal)0.5;
        }

        public DraftDto(int Id, string Location, string Abb, string Mascot, string UserUsername, DateTime Date, bool? Complete,
            int? Rounds, decimal? Speed)
            : base(Id, Location, Abb, Mascot, UserUsername, Date, Complete)
        {
            this.Rounds = Rounds ?? 3;
            this.Speed = Speed ?? (decimal)0.5;
        }
        #endregion Constructors
    }
}
