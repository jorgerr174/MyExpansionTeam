using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace METCore.Models.Players
{
    public class Prospect : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        public int Year { get; set; }

        public int Consensus { get; set; }

        public string? HandSize { get; set; }

        public string? ArmLength { get; set; }

        public string? Wingspan { get; set; }

        public string? FortyYardDash { get; set; }

        public string? BenchPress { get; set; }

        public string? VertJump { get; set; }

        public string? BroadJump { get; set; }

        public string? ThreeConeDrill { get; set; }

        public string? TwentyYardShuttle { get; set; }

        public int AthScore { get; set; }


        [NotMapped]
        public string[] ImportAttributes =>
            new string?[] { HandSize, ArmLength, Wingspan, FortyYardDash, BenchPress, VertJump, BroadJump,
                ThreeConeDrill, TwentyYardShuttle, AthScore.ToString()}
                .Select(s => s ?? string.Empty).ToArray();
        #endregion Attributes


        #region Constructors
        public Prospect() : base() { }
        #endregion Constructors
    }
}
