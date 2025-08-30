using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using METCore.ValidationAttributes;

namespace METCore.DTOs.Team
{
    public class TeamInfoDto : TeamBasicInfoDto
    {
        #region RosterSettings

        [Range(60, 120, ErrorMessage = "Número debe ser entre 60% y 120%")]
        [DisplayName("Límite Salarial (%)")]
        public int RosterSettingsCap { get; set; }

        [Range(2, 10, ErrorMessage = "Número debe ser entre 2 y 10")]
        [DisplayName("Máx Jugadores Por Equipo")]
        public int RosterSettingsMaxPerTeam { get; set; }

        [Range(0, 6, ErrorMessage = "Número debe ser entre 0 y 6")]
        [DisplayName("Protegidos por equipo")]
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
