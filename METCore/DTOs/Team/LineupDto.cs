namespace METCore.DTOs.Team
{
    public class LineupDto : SPLineupDto
    {
        #region Attributes
        public int Player6 { get; set; }
        public int Player7 { get; set; }
        public int Player8 { get; set; }
        public int Player9 { get; set; }
        public int Player10 { get; set; }
        public int Player11 { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public LineupDto() : base() { }
        #endregion Constructors
    }
}
