namespace METCore.DTOs.Team
{
    public class TeamDto : TeamInfoDto
    {
        #region Attributes
        public IList<int> PlayersIds { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public TeamDto() : base()
        {
            this.PlayersIds = [];
        }

        public TeamDto(int Id, string Location, string Abb, string Mascot, string UserUsername, DateTime Date, bool? Complete,
            int? Cap, int? MaxPerTeam, int? ProtectedPerTeam, IList<int>? ProtectedPlayersIds = null, IList<int>? PlayersIds = null)
            : base(Id, Location, Abb, Mascot, UserUsername, Date, Complete, Cap, MaxPerTeam, ProtectedPerTeam, ProtectedPlayersIds)
        {
            this.PlayersIds = PlayersIds ?? [];
        }
        #endregion Constructors
    }
}
