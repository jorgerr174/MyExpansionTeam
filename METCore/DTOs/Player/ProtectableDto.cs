namespace METCore.DTOs.Player
{
    public class ProtectableDto : PlayerBasicDto
    {
        public string Height { get; set; }

        public string Weight { get; set; }

        public string? Age { get; set; }

        public bool DefaultProtected { get; set; }

        public int Madden { get; set; }


        public ProtectableDto() : base()
        {
            this.Height = string.Empty;
            this.Weight = string.Empty;
            this.DefaultProtected = false;
        }
    }
}
