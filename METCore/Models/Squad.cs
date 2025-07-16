using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models
{
    public abstract class Squad : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        [MaxLength(50)]
        public string Location { get; set; }

        [MaxLength(50)]
        public string Mascot { get; set; }

        [NotMapped]
        public string FullName { get { return this.Location + " " + this.Mascot; } }
        #endregion Attributes


        #region Constructors
        public Squad()
        {
            this.Location = "";
            this.Mascot = "";
        }

        public Squad(string Location, string Mascot)
        {
            this.Location = Location;
            this.Mascot = Mascot;
        }
        #endregion
    }
}
