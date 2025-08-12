using System.ComponentModel.DataAnnotations;
using METCore.DTOs.Player;
using static METCore.Enums.Types;

namespace METCore.DTOs.Team
{
    public class DraftDto : TeamBasicInfoDto
    {
        #region Attributes
        [Required]
        [Range(0, 7)]
        public int Rounds { get; set; }

        [Required]
        public IDictionary<int, int>? Selections { get; set; }

        public IList<IList<int>> Picks { get; set; }

        public IList<ProspectDto> Prospects { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public DraftDto() : base()
        {
            this.Rounds = 3;
            this.Picks = DraftPicks.GetAllPicks();
            this.Prospects = [];
        }

        public DraftDto(int Id, string Location, string Abb, string Mascot, string UserUsername, DateTime Date, bool? Complete,
            int? Rounds, IDictionary<int, int>? Selections)
            : base(Id, Location, Abb, Mascot, UserUsername, Date, Complete)
        {
            this.Rounds = Rounds ?? 3;
            this.Selections = Selections;
            this.Picks = DraftPicks.GetAllPicks();
            this.Prospects = [];
        }
        #endregion Constructors
    }
}
