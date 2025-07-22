using METCore.DTOs.Player;

namespace METCore.DTOs.Team
{
    public class TeamDto : TeamInfoDto
    {
        #region Attributes
        public IList<RosteredDto> Players { get; set; }

        public virtual LineupDto OffLineup { get; set; }
        public virtual LineupDto DefLineup { get; set; }
        public virtual SPLineupDto SPLineup { get; set; }

        public IList<int> SelectedIds { get; set; }

        public IList<int> Picks { get; set; }

        public IList<PlayerBasicDto> TradedPlayers { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public TeamDto() : base()
        {
            this.Players = [];
            this.SelectedIds = [];
            this.Picks = [];
            this.TradedPlayers = [];
            this.OffLineup = new();
            this.DefLineup = new();
            this.SPLineup = new();
        }
        #endregion Constructors
    }
}
