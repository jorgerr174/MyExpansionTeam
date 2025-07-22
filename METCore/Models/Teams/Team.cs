using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using METCore.DTOs.Team;
using METCore.Models.Players;

namespace METCore.Models.Teams
{
    public class Team : Squad
    {
        #region Attributes
        public virtual User User { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [MinLength(2)]
        [MaxLength(3)]
        public string Abb { get; set; }

        [DefaultValue(false)]
        public bool Complete { get; set; }

        public RosterSettings? RosterSettings { get; set; }

        public virtual IList<int> PlayersIds { get; set; }
        
        public virtual Lineup OffLineup { get; set; }
        public virtual Lineup DefLineup { get; set; }
        public virtual SPLineup SPLineup { get; set; }

        public virtual IList<int> ProtectedPlayersIds { get; set; }

        public virtual IList<Trade>? Trades { get; set; }

        public IDictionary<int, int>? Selections { get; set; }

        #region Not Mapped
        [NotMapped]
        public virtual IList<Player> Players { get; set; }

        [NotMapped]
        public virtual double CurrentCap => this.Players.Sum(p => p.ActiveContract.APY);
        #endregion Not Mapped
        #endregion Attributes


        #region Constructors
        public Team() : base()
        {
            this.User = new();
            this.Date = DateTime.Now;
            this.Abb = "";
            this.RosterSettings = null;
            this.PlayersIds = [];
            this.OffLineup = new();
            this.DefLineup = new();
            this.SPLineup = new();
            this.ProtectedPlayersIds = [];
            this.Players = [];
        }
        #endregion Constructors
    }
}
