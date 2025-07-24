using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Stats
{
    public class FGStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int ShMade { get; set; }
        public int ShAtt { get; set; }

        public int MidMade { get; set; }
        public int MidAtt { get; set; }

        public int LongMade { get; set; }
        public int LongAtt { get; set; }

        public int Blk { get; set; }

        public int Lng { get; set; }

        #region NotMapped
        [NotMapped]
        public double Pct { get { return Att == 0 ? 0 : Made / Att; } }

        [NotMapped]
        public int Made { get { return ShMade + MidMade + LongMade; } }

        [NotMapped]
        public int Att { get { return ShAtt + MidAtt + LongAtt; } }
        #endregion NotMapped

        #endregion Attributes


        #region Constructors
        public FGStats() { }
        #endregion Constructors
    }
}
