using METCore.Interfaces.Importing;
using static METCore.Enums.Types;

namespace METCore.DTOs.Player
{
    public abstract class ImportAthleteDto : IImportableDto
    {
        public string Player { get; set; }

        public int? Height { get; set; }

        public int? Weight { get; set; }

        public PositionEnum? Position { get; set; }

        public string? College { get; set; }


        public ImportAthleteDto()
        {
            this.Player = string.Empty;
        }

        public ImportAthleteDto(string Player, int Height, int Weight, string College, PositionEnum Position)
        {
            this.Player = Player;
            this.Height = Height;
            this.Weight = Weight;
            this.Position = Position;
            this.College = College;
        }
    }
}
