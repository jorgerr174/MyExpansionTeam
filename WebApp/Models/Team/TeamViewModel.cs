/*
using System.ComponentModel.DataAnnotations;
using METCore.DTOs.Player;

namespace WebApp.Models.Team
{
    public class TeamViewModel
    {
        public int? Id { get; set; } = 0;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(3)]
        public string Abb { get; set; } = string.Empty;

        [Required]
        public string Mascot { get; set; } = string.Empty;

        public IList<PlayerDto>? Players { get; set; } = null;

        [Required]
        public string UserUsername { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public bool Complete { get; set; } = false;


        #region RosterSettings
        [Range(0.6, 1)]
        public decimal RosterSettingsCap { get; set; } = (decimal)0.80;

        [Range(1, 6)]
        public int RosterSettingsMaxPerTeam { get; set; } = 3;

        [Range(1, 6)]
        public int RosterSettingsBlockedPerTeam { get; set; } = 3;
        #endregion RosterSettings
    }
}
*/
