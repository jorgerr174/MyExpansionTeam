using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Stats
{
    public class IntStats : BaseClass, IStats
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int INT { get; set; }

        public int TD { get; set; }

        public int Yds { get; set; }

        public int Lng { get; set; }

        #region NotMapped
        #endregion NotMapped
        #endregion Attributes


        #region Constructors
        public IntStats() { }

        public IntStats(int INT, int TD, int Yds, int Lng)
        {
            this.INT = INT;
            this.TD = TD;
            this.Yds = Yds;
            this.Lng = Lng;
        }
        #endregion Constructors
    }
}
