using System.ComponentModel.DataAnnotations;
using METCore.Models.Stats;

namespace METCore.Models
{
    public class SeasonStats : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int Season { get; set; }

        //public int Games { get; set; }
        public virtual PassStats? PassStats { get; set; }
        public virtual RecStats? RecStats { get; set; }
        public virtual RushStats? RushStats { get; set; }

        public virtual IntStats? IntStats { get; set; }
        public virtual TackleStats? TackleStats { get; set; }

        public virtual KOStats? KOStats { get; set; }
        public virtual KRStats? KRStats { get; set; }
        public virtual PuntStats? PuntStats { get; set; }
        public virtual PRStats? PRStats { get; set; }
        public virtual FGStats? FGStats { get; set; }
        #endregion Attributes


        #region Constructors
        public SeasonStats(int season)
        {
            this.Season = season;
        }

        public SeasonStats(int Season, PassStats? PassStats, RecStats? RecStats, RushStats? RushStats,
            TackleStats? TackleStats, IntStats? IntStats,
            KOStats? KOStats, KRStats? KRStats, PuntStats? PuntStats, PRStats? PRStats, FGStats? FGStats)
        {
            this.Season = Season;

            this.PassStats = PassStats;
            this.RecStats = RecStats;
            this.RushStats = RushStats;

            this.TackleStats = TackleStats;
            this.IntStats = IntStats;

            this.KOStats = KOStats;
            this.KRStats = KRStats;
            this.PuntStats = PuntStats;
            this.PRStats = PRStats;
            this.FGStats = FGStats;
        }
        #endregion Constructors
    }
}
