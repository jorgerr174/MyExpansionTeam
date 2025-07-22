namespace METCore.Models.Teams
{
    public class Lineup : SPLineup
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
        public Lineup() : base() { }
        #endregion Constructors
    }
}
