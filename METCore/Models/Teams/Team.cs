using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual IList<int> ProtectedPlayersIds { get; set; }

        public virtual IList<Trade>? Trades { get; set; }

        public string? Draft { get; set; }

        #region Not Mapped
        [NotMapped]
        public virtual IList<Player> Players { get; set; }
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
            this.ProtectedPlayersIds = [];
            this.Players = [];
        }

        public Team(string Location, string Mascot, List<Player> Players, User User, DateTime Date, string Abb) : base(Location, Mascot)
        {
            this.User = User;
            this.Date = Date;
            this.Abb = Abb;
            this.RosterSettings = null;
            this.PlayersIds = [];
            this.ProtectedPlayersIds = [];
            this.Players = Players ?? [];
        }

        public Team(string Location, string Mascot, List<Player> Players, User User, DateTime Date, string Abb,
            bool? Complete, decimal? Cap, int? MaxPerTeam, int? ProtectedPerTeam, IList<int>? ProtectedPlayersIds, IList<Trade>? Trades, string? Draft)
            : this(Location, Mascot, Players, User, Date, Abb)
        {
            this.Complete = Complete.HasValue && Complete.Value;
            this.RosterSettings = Cap.HasValue || MaxPerTeam.HasValue || ProtectedPerTeam.HasValue ? new(Cap, MaxPerTeam, ProtectedPerTeam) : null;
            this.ProtectedPlayersIds = ProtectedPlayersIds ?? [];
            this.Trades = Trades;
            this.Draft = Draft;
        }
        #endregion Constructors
    }
}
