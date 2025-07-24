using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Stats
{
    #region KOStats
    public class KOStats : KickStats
    {
        #region Attributes
        public int OnSide { get; set; }

        public int OnSideRec { get; set; }

        #region NotMapped
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public KOStats() { }
        #endregion Constructors
    }
    #endregion KOStats


    #region PuntStats
    public class PuntStats : KickStats
    {
        #region Attributes
        public int Inside20 { get; set; }

        public int Down { get; set; }

        public int FC { get; set; }

        public int Blk { get; set; }

        public int Lng { get; set; }

        #region NotMapped
        [NotMapped]
        public double Avg { get { return Kick == 0 ? 0 : Yds / Kick; } }

        [NotMapped]
        public double NetAvg { get { return Avg - RetAvg; } }
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public PuntStats() { }
        #endregion Constructors
    }
    #endregion PuntStats


    #region KickStats
    public abstract class KickStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int Kick { get; set; }

        public int Yds { get; set; }

        public int TB { get; set; }

        public int OOB { get; set; }

        public int Ret { get; set; }

        public int RetYds { get; set; }

        public int TD { get; set; }

        #region NotMapped
        [NotMapped]
        public double TBPct { get { return Kick == 0 ? 0 : TB / Kick; } }

        [NotMapped]
        public double RetAvg { get { return Ret == 0 ? 0 : RetYds / Ret; } }
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public KickStats() { }
        #endregion Constructors
    }
    #endregion KickStats
}