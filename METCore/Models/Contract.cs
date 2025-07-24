using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using METCore.Interfaces.Importing;

namespace METCore.Models
{
    public class Contract : BaseClass, IImportable
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int FranchiseId { get; set; }

        public int YearSigned { get; set; }

        public int Length { get; set; }

        public long Total { get; set; }

        public long Guaranteed { get; set; }

        public bool Active { get; set; }

        #region Not Mapped
        [NotMapped]
        public double APY { get { return this.Total / this.Length; } }

        [NotMapped]
        public int LastSeason { get { return this.YearSigned + this.Length; } }
        #endregion

        #endregion


        #region Constructors
        public Contract() { }
        #endregion
    }
}
