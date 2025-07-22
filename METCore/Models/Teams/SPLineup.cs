using METCore.Interfaces;

namespace METCore.Models.Teams
{
    public class SPLineup : ILineup
    {
        #region Attributes
        public string Formation { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int Player3 { get; set; }
        public int Player4 { get; set; }
        public int Player5 { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public SPLineup()
        {
            this.Formation = string.Empty;
        }
        #endregion Constructors
    }
}
