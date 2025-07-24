using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Stats
{
    #region PassStats
    public class PassStats : SkillStats
    {
        #region Attributes
        public int Cmp { get; set; }

        public int INT { get; set; }

        public double PR { get; set; }

        public int Sck { get; set; }

        public int SckYds { get; set; }
        #region NotMapped
        [NotMapped]
        public double CmpPct { get { return Att == 0 ? 0 : Cmp / Att; } }

        [NotMapped]
        public double Y_S { get { return Sck == 0 ? 0 : SckYds / Sck; } }
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public PassStats() { }
        #endregion Constructors
    }
    #endregion PassStats


    #region RecStats
    public class RecStats : SkillStats
    {
        #region Attributes
        public int Rec { get; set; }

        public int Fmb { get; set; }

        public int YAC { get; set; }
        #region NotMapped
        [NotMapped]
        public double R_A { get { return Att == 0 ? 0 : Rec / Att; } }
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public RecStats() { }
        #endregion Constructors
    }
    #endregion RecStats


    #region RushStats
    public class RushStats : SkillStats
    {
        #region Attributes
        public int Fmb { get; set; }
        #region NotMapped
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public RushStats() { }
        #endregion Constructors
    }
    #endregion RushStats


    #region SkillStats
    public abstract class SkillStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int Yds { get; set; }

        public int Att { get; set; }

        public int TD { get; set; }

        public int Plus20 { get; set; }

        public int Plus40 { get; set; }

        public int Reach1st { get; set; }

        public int Lng { get; set; }
        #region NotMapped
        [NotMapped]
        public double Y_A { get { return Att == 0 ? 0 : Yds / Att; } }

        [NotMapped]
        public double Reach1stPct { get { return Att == 0 ? 0 : Reach1st / Att; } }
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public SkillStats() { }
        #endregion Constructors
    }
    #endregion SkillStats
}