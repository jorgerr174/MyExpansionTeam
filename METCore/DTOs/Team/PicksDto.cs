using static METCore.Enums.Types;

namespace METCore.DTOs.Team
{
    public class PicksDto
    {
        #region Attributes
        public int TeamId { get; set; }
        public IList<int>[] Picks { get; set; }
        #endregion Attributes


        #region Constructors
        public PicksDto()
        {
            this.Picks = DraftPicks.GetAllPicks();
        }

        public PicksDto(int TeamId) : this()
        {
            this.TeamId = TeamId;
        }

        public PicksDto(int TeamId, IList<int>[] Picks)
        {
            this.TeamId = TeamId;
            this.Picks = Picks;
        }
        #endregion Constructors
    }
}
