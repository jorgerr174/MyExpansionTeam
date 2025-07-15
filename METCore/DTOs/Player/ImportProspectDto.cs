using static METCore.Enums.Types;

namespace METCore.DTOs.Player
{
    public class ImportProspectDto : ImportAthleteDto
    {
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


        public ImportProspectDto() : base() { }

        public ImportProspectDto(string Player, int Height, int Weight, string College, PositionEnum Position,
            int Year, int Consensus, string? HandSize, string? ArmLength, string? Wingspan, string? FortyYardDash, string? BenchPress,
            string? VertJump, string? BroadJump, string? ThreeConeDrill, string? TwentyYardShuttle, int AthScore)
            : base(Player, Height, Weight, College, Position)
        {
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
    }
}
