namespace METCore.DTOs.Player
{
    public class PlayerDto : AthleteDto
    {
        public DateOnly? BirthDate { get; set; }


        public PlayerDto() { }

        public PlayerDto(int Id, string Name, int Height, int Weight, string College, string Position, DateOnly BirthDate)
            : base(Id, Name, Height, Weight, College, Position)
        {
            this.BirthDate = BirthDate;
        }
    }
}
