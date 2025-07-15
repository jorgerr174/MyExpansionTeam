using System.ComponentModel.DataAnnotations;
using METCore.ValidationAttributes;

namespace METCore.DTOs.Team
{
    public class TeamInfoDto : TeamBasicInfoDto
    {
        #region RosterSettings

        [Display(Name = "Salary Cap Percentage")]
        [Range(60, 120, ErrorMessage = "Salary cap percentage must be between 60% and 120%")]
        public int RosterSettingsCap { get; set; }

        [Display(Name = "Max # of Robbed players per Franchise")]
        [Range(2, 10, ErrorMessage = "Must be between 2 and 10")]
        public int RosterSettingsMaxPerTeam { get; set; }

        [Display(Name = "Protected Players per Franchise")]
        [Range(0, 6, ErrorMessage = "# of Protected Players per Franchise must be between 0 and 6")]
        public int RosterSettingsProtectedPerTeam { get; set; }

        [ProtectedPlayersValidation]
        public IList<int> RosterSettingsProtectedPlayersIds { get; set; }
        #endregion RosterSettings


        #region Constructors
        public TeamInfoDto() : base()
        {
            this.RosterSettingsCap = 80;
            this.RosterSettingsMaxPerTeam = 3;
            this.RosterSettingsProtectedPerTeam = 3;
            this.RosterSettingsProtectedPlayersIds = [];
        }

        public TeamInfoDto(int Id, string Location, string Abb, string Mascot)
            : base(Id, Location, Abb, Mascot)
        {
            this.RosterSettingsCap = 80;
            this.RosterSettingsMaxPerTeam = 3;
            this.RosterSettingsProtectedPerTeam = 3;
            this.RosterSettingsProtectedPlayersIds = [];
        }

        public TeamInfoDto(int Id, string Location, string Abb, string Mascot, string UserUsername, DateTime Date, bool? Complete,
                int? Cap, int? MaxPerTeam, int? ProtectedPerTeam, IList<int>? ProtectedPlayersIds = null)
            : base(Id, Location, Abb, Mascot, UserUsername, Date, Complete)
        {
            this.RosterSettingsCap = Cap ?? 80;
            this.RosterSettingsMaxPerTeam = MaxPerTeam ?? 3;
            this.RosterSettingsProtectedPerTeam = ProtectedPerTeam ?? 3;
            this.RosterSettingsProtectedPlayersIds = ProtectedPlayersIds ?? [];
        }
        #endregion Constructors
    }
}
