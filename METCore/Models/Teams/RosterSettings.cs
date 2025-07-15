using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Teams
{
    public class RosterSettings
    {
        #region Attributes
        [Required]
        [Range(0.6, 1.1)]
        public decimal? Cap { get; set; }

        [Required]
        [Range(2, 10)]
        public int? MaxPerTeam { get; set; }

        [Required]
        [Range(0, 6)]
        public int? ProtectedPerTeam { get; set; }

        public IList<int> ProtectedPlayersIds { get; set; }
        #endregion Attributes


        #region Constructors
        public RosterSettings()
        {
            Cap = (decimal)0.80;
            MaxPerTeam = 3;
            ProtectedPerTeam = 3;
            ProtectedPlayersIds = [];
        }

        public RosterSettings(decimal? Cap, int? MaxPerTeam, int? ProtectedPerTeam)
        {
            this.Cap = Cap;
            this.MaxPerTeam = MaxPerTeam;
            this.ProtectedPerTeam = ProtectedPerTeam;
            ProtectedPlayersIds = [];
        }

        public RosterSettings(decimal? Cap, int? MaxPerTeam, int? ProtectedPerTeam, List<int> ProtectedPlayersIds) : this(Cap, MaxPerTeam, ProtectedPerTeam)
        {
            this.ProtectedPlayersIds = ProtectedPlayersIds;
        }
        #endregion Constructors
    }
}
