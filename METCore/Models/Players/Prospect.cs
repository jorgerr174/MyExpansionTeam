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

        public Prospect(int Id, int Year, int Consensus,
            string? HandSize, string? ArmLength, string? Wingspan, string? FortyYardDash, string? BenchPress,
            string? VertJump, string? BroadJump, string? ThreeConeDrill, string? TwentyYardShuttle, int AthScore)
        {
            this.Id = Id;
            this.Year = Year;
            this.Consensus = Consensus;
            this.HandSize = HandSize;
            this.ArmLength = ArmLength;
            this.Wingspan = Wingspan;
            this.FortyYardDash = FortyYardDash;
            this.BenchPress = BenchPress;
            this.VertJump = VertJump;
            this.BroadJump = BroadJump;
            this.ThreeConeDrill = ThreeConeDrill;
            this.TwentyYardShuttle = TwentyYardShuttle;
            this.AthScore = AthScore;
        }
        #endregion Constructors
    }
}
