using static METCore.Enums.Types;

namespace METCore.DTOs.Player
{
    public class AthleteDto
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? Height { get; set; }

        public int? Weight { get; set; }

        public string? Position { get; set; }

        public string? College { get; set; }


        public AthleteDto() { }

        public AthleteDto(int Id, string Name, int Height, int Weight, string College, string Position)
        {
            this.Id = Id;
            this.Name = Name;
            this.Height = Height;
            this.Weight = Weight;
            this.Position = Position;
            this.College = College;
        }
    }
}
