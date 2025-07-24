using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Stats
{
    #region KickReturn
    public class KRStats : ReturnStats
    {
        #region Attributes

        #region NotMapped
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public KRStats() { }
        #endregion Constructors
    }
    #endregion KickReturn


    #region PRStats
    public class PRStats : ReturnStats
    {
        #region Attributes

        #region NotMapped
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public PRStats() { }
        #endregion Constructors
    }
    #endregion PRStats


    #region ReturnStats
    public abstract class ReturnStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int Ret { get; set; }

        public int Yds { get; set; }

        public int TD { get; set; }

        public int Plus20 { get; set; }

        public int Plus40 { get; set; }

        public int Lng { get; set; }

        public int FC { get; set; }

        public int Fmb { get; set; }

        #region NotMapped
        [NotMapped]
        public double Avg { get { return Ret == 0 ? 0 : Yds / Ret; } }
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public ReturnStats() { }
        #endregion Constructors
    }
    #endregion ReturnStats
}
