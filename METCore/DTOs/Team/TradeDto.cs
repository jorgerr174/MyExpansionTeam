using METCore.DTOs.Player;

namespace METCore.DTOs.Team
{
    public class TradeDto
    {
        #region Attributes
        public int Id { get; set; }
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


        public TradeDto(int Id, int TeamId, int FranchiseId, bool? Force, int TeamCurrentCap, IList<SelectableDto> TeamPlayers, IList<int> TeamPicks, IList<SelectableDto> FranchisePlayers, IList<int> FranchisePicks)
        {
            this.Id = Id;
            this.TeamId = TeamId;
            this.FranchiseId = FranchiseId;
            this.Force = Force.HasValue && Force.Value;
            this.TeamCurrentCap = TeamCurrentCap;

            this.TeamPlayers = TeamPlayers ?? [];
            this.TeamPicks = TeamPicks ?? [];

            this.FranchisePlayers = FranchisePlayers ?? [];
            this.FranchisePicks = FranchisePicks ?? [];
        }
        #endregion Constructors
    }
}
