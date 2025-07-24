using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Stats
{
    public class TackleStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }
        public int Comb { get; set; }
        public int Solo { get; set; }
        public int Asst { get; set; }
        public double Sck { get; set; }
        #region NotMapped
        [NotMapped]
        public double Total => Solo + Comb;
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public TackleStats() { }
        #endregion Constructors
    }
}
