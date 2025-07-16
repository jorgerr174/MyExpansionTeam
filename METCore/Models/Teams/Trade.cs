using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Teams
{
    public class Trade : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }
        public int TeamId { get; set; }
        public int FranchiseId { get; set; }
        public bool Force { get; set; }
        public int TeamCurrentCap { get; set; }

        public IList<int> TeamPlayers { get; set; }
        public IList<int> TeamPicks { get; set; }

        public IList<int> FranchisePlayers { get; set; }
        public IList<int> FranchisePicks { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public Trade()
        {
            this.Force = false;
            this.TeamPlayers = [];
            this.TeamPicks = [];
            this.FranchisePlayers = [];
            this.FranchisePicks = [];
        }

        public Trade(int Id, int TeamId, int FranchiseId, bool Force, int TeamCurrentCap, IList<int> TeamPlayers, IList<int> TeamPicks, IList<int> FranchisePlayers, IList<int> FranchisePicks)
        {
            this.Id = Id;
            this.TeamId = TeamId;
            this.FranchiseId = FranchiseId;
            this.Force = Force;
            this.TeamCurrentCap = TeamCurrentCap;

            this.TeamPlayers = TeamPlayers ?? [];
            this.TeamPicks = TeamPicks ?? [];

            this.FranchisePlayers = FranchisePlayers ?? [];
            this.FranchisePicks = FranchisePicks ?? [];
        }
        #endregion Constructors
    }
}
