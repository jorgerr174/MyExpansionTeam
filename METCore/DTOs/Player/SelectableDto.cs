namespace METCore.DTOs.Player
{
    public class SelectableDto : ProtectableDto
    {
        public string PureAPY { get; set; }


        public SelectableDto() : base()
        {
            this.PureAPY = string.Empty;
        }
    }
}
