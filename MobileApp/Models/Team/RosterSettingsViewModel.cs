using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class RosterSettingsViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;
        private readonly AccountService _accountService;

        public RosterSettingsViewModel(TeamService teamService, AccountService accountService)
        {
            _teamService = teamService;
            _accountService = accountService;
        }

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private int rosterSettingsCap = 80;
        [ObservableProperty] private int rosterSettingsMaxPerTeam = 3;
        [ObservableProperty] private int rosterSettingsProtectedPerTeam = 3;
        [ObservableProperty] private List<int> rosterSettingsProtectedPlayersIds = [];
        [ObservableProperty] private IList<SelectablePlayerViewModel> protectablePlayers = [];
        [ObservableProperty] private bool showPlayerSelection = false;

        [RelayCommand]
        public async Task LoadRosterSettings(int id)
        {
            TeamId = id;
            IsLoading = true;
            ShowPlayerSelection = false;

            try
            {
                var team = await _teamService.GetTeamDetailsAsync(id);
                if (team != null)
                {
                    TeamName = $"{team.Location} {team.Mascot}";
                    RosterSettingsCap = team.RosterSettingsCap;
                    RosterSettingsMaxPerTeam = team.RosterSettingsMaxPerTeam;
                    RosterSettingsProtectedPerTeam = team.RosterSettingsProtectedPerTeam;
                    RosterSettingsProtectedPlayersIds = team.RosterSettingsProtectedPlayersIds.ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load roster settings: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadProtectablePlayers()
        {
            IsLoading = true;
            try
            {
                var players = await _teamService.GetProtectablePlayersAsync(TeamId);
                if (players != null)
                {
                    var wrappedPlayers = players.Select(p =>
                    {
                        var wrapper = new SelectablePlayerViewModel(p);
                        wrapper.IsSelected = RosterSettingsProtectedPlayersIds.Contains(p.Id);
                        return wrapper;
                    }).ToList();

                    ProtectablePlayers = wrappedPlayers;
                    ShowPlayerSelection = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load players: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void TogglePlayerSelection(SelectablePlayerViewModel playerWrapper)
        {
            var selectedCount = ProtectablePlayers.Count(p => p.IsSelected);

            if (!playerWrapper.IsSelected && selectedCount >= RosterSettingsProtectedPerTeam)
            {
                ErrorMessage = $"You can only protect {RosterSettingsProtectedPerTeam} players";
                return;
            }

            playerWrapper.IsSelected = !playerWrapper.IsSelected;

            // Update the protected players IDs list
            RosterSettingsProtectedPlayersIds = [.. ProtectablePlayers.Where(p => p.IsSelected).Select(p => p.Id)];

            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        public void HidePlayerSelection()
        {
            ShowPlayerSelection = false;
        }

        [RelayCommand]
        public async Task SaveRosterSettings()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var teamDto = new TeamInfoDto(TeamId, "", "", "")
                {
                    RosterSettingsCap = RosterSettingsCap,
                    RosterSettingsMaxPerTeam = RosterSettingsMaxPerTeam,
                    RosterSettingsProtectedPerTeam = RosterSettingsProtectedPerTeam,
                    RosterSettingsProtectedPlayersIds = RosterSettingsProtectedPlayersIds
                };

                bool success = await _teamService.UpdateRosterSettingsAsync(teamDto);

                if (success)
                {
                    await Shell.Current.GoToAsync("MyTeams");
                }
                else
                {
                    ErrorMessage = "Failed to save roster settings";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Save failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}