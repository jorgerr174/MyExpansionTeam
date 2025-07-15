using static METCore.Enums.Types;

namespace METCore.DTOs.Player
{
    public class ImportPlayerDto : ImportAthleteDto
    {
        public DateOnly? BirthDate { get; set; }


        public ImportPlayerDto() : base() { }

        public ImportPlayerDto(string Player, int Height, int Weight, string College, PositionEnum Position,
                DateOnly BirthDate)
            : base(Player, Height, Weight, College, Position)
        {
            this.BirthDate = BirthDate;
        }
    }
}
