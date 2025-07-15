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

        public KRStats(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb) : base(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb) { }
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

        public PRStats(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb) : base(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb) { }
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

        public ReturnStats(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
        {
            this.Ret = Ret;
            this.Yds = Yds;
            this.TD = TD;
            this.Plus20 = Plus20;
            this.Plus40 = Plus40;
            this.Lng = Lng;
            this.FC = FC;
            this.Fmb = Fmb;
        }
        #endregion Constructors
    }
    #endregion ReturnStats
}
