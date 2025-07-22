namespace METCore.DTOs.Player
{
    public class PlayerBasicDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string APY { get; set; }


        public PlayerBasicDto()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.Position = string.Empty;
            this.APY = string.Empty;
        }
    }
}
