namespace METCore.DTOs.Player
{
    public class SelectableDto : ProtectableDto
    {
        public string PureAPY { get; set; }

        public string Stats { get; set; }

        public SelectableDto() : base()
        {
            this.PureAPY = string.Empty;
            this.Stats = string.Empty;
        }
    }
}
