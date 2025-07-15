namespace METCore.DTOs.Team
{
    public class UserPicksDto
    {
        #region Attributes
        public int TeamId { get; set; }
        public IList<int> TeamPicks { get; set; }
        public IList<int> OthersPicks { get; set; }
        public int Rounds { get; set; }
        #endregion Attributes


        #region Constructors
        public UserPicksDto() : base()
        {
            this.TeamPicks = [];
            this.OthersPicks = [];
        }

        public UserPicksDto(int TeamId, IList<int> TeamPicks, IList<int> OthersPicks, int Rounds)
        {
            this.TeamId = TeamId;
            this.TeamPicks = TeamPicks;
            this.OthersPicks = OthersPicks;
            this.Rounds = Rounds;
        }
        #endregion Constructors
    }
}
