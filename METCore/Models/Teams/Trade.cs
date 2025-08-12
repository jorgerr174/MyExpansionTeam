using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Teams
{
    public class Trade : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }
        public DateOnly Date { get; set; }

        public int TeamId { get; set; }
        public int FranchiseId { get; set; }

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
            this.TeamPlayers = [];
            this.TeamPicks = [];
            this.FranchisePlayers = [];
            this.FranchisePicks = [];
        }
        #endregion Constructors
    }
}
