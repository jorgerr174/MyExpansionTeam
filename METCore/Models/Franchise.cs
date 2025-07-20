using System.ComponentModel.DataAnnotations.Schema;
using METCore.Models.Players;
using static METCore.Enums.Types;

namespace METCore.Models
{
    public class Franchise : Squad
    {
        #region Attributes
        public virtual IList<Player> Players { get; set; }

        public virtual Player Protected1 { get; set => this.SetProtected(true, value); }

        public virtual Player Protected2 { get; set => this.SetProtected(false, value); }

        public virtual Player Protected3 { get; set => this.SetProtected(null, value); }

        public void SetProtected(bool? flag, Player player)
        {
            this.Protected3 = flag.HasValue ? this.Protected2 : player;
            this.Protected2 = !flag.HasValue ? this.Protected2 : !flag.Value ? player : this.Protected1;
            this.Protected1 = (!flag.HasValue || !flag.Value) ? this.Protected1 : player;
        }

        [NotMapped]
        public virtual IList<Player> ProtectedPlayers => [this.Protected1, this.Protected2, this.Protected3];

        [NotMapped]
        public virtual IList<Player> PlayersToProtect => [.. this.ProtectedPlayers.Concat(this.Players).Distinct()];

        [NotMapped]
        public virtual IList<Player> PlayersByPosition => [.. this.PlayersToProtect.OrderBy(p => (int)p.Position)];

        [NotMapped]
        public virtual FranchiseEnum Abb { get { return (FranchiseEnum)this.Id; } }
        #endregion Attributes


        #region Constructors
        public Franchise() : base()
        {
            this.Players = [];
        }

        public Franchise(string Location, string Mascot, IList<Player>? Players) : base(Location, Mascot)
        {
            this.Players = Players ?? [];
        }
        #endregion Constructors
    }
}
