using System.ComponentModel.DataAnnotations;

namespace METCore.DTOs.Team
{
    public class RosterSettingsDto
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        [Required]
        [Range(0.6, 1.1, ErrorMessage = "Cap must be between 60% and 110%")]
        public decimal? Cap { get; set; }

        [Required]
        [Range(2, 10, ErrorMessage = "Max per team must be between 2 and 10")]
        public int? MaxPerTeam { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Protected per team must be between 0 and 6")]
        public int? ProtectedPerTeam { get; set; }

        public IList<int> ProtectedPlayersIds { get; set; }

        public RosterSettingsDto()
        {
            this.TeamId = 0;
            this.TeamName = string.Empty;
            this.Cap = 0;
            this.MaxPerTeam = 3;
            this.ProtectedPerTeam = 3;
            this.ProtectedPlayersIds = [];
        }

        public RosterSettingsDto(int TeamId, string TeamName, decimal Cap = (decimal)0.8, int MaxPerTeam = 3, int ProtectedPerTeam = 3, IList<int>? ProtectedPlayersIds = null)
        {
            this.TeamId = TeamId;
            this.TeamName = TeamName;
            this.Cap = Cap;
            this.MaxPerTeam = MaxPerTeam;
            this.ProtectedPerTeam = ProtectedPerTeam;
            this.ProtectedPlayersIds = ProtectedPlayersIds ?? [];
        }
    }
}
