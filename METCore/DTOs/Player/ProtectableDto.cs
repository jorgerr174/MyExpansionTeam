namespace METCore.DTOs.Player
{
    public class ProtectableDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Height { get; set; }

        public string Weight { get; set; }

        public string? Age { get; set; }

        public string Position { get; set; }

        public string APY { get; set; }

        public bool DefaultProtected { get; set; }


        public ProtectableDto()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.Height = string.Empty;
            this.Weight = string.Empty;
            this.Position = string.Empty;
            this.APY = string.Empty;
            this.DefaultProtected = false;
        }
    }
}
