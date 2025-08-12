using METCore.DTOs.Player;

namespace METCore.DTOs.Team
{
    public class TradeDto
    {
        #region Attributes
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int TeamId { get; set; }
        public int FranchiseId { get; set; }
        public bool Force { get; set; }
        public decimal TeamCurrentCap { get; set; }

        public IList<SelectableDto> TeamPlayers { get; set; }
        public IList<int> TeamPicks { get; set; }

        public IList<SelectableDto> FranchisePlayers { get; set; }
        public IList<int> FranchisePicks { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public TradeDto()
        {
            this.Force = false;
            this.TeamPlayers = [];
            this.TeamPicks = [];
            this.FranchisePlayers = [];
            this.FranchisePicks = [];
        }

        public TradeDto(int TeamId, int FranchiseId)
        {
            this.TeamId = TeamId;
            this.FranchiseId = FranchiseId;
            this.Force = false;
            this.TeamPlayers = [];
            this.TeamPicks = [];
            this.FranchisePlayers = [];
            this.FranchisePicks = [];
        }
        #endregion Constructors
    }
}
