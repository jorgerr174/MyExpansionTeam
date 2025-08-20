using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class RosterSettingsViewModel(TeamService teamService, AccountService accountService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        private readonly AccountService _accountService = accountService;

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
                if (await _teamService.GetTeamDetailsAsync(id) is TeamInfoDto team)
                {
                    TeamName = $"{team.Location} {team.Mascot}";
                    RosterSettingsCap = team.RosterSettingsCap;
                    RosterSettingsMaxPerTeam = team.RosterSettingsMaxPerTeam;
                    RosterSettingsProtectedPerTeam = team.RosterSettingsProtectedPerTeam;
                    RosterSettingsProtectedPlayersIds = [.. team.RosterSettingsProtectedPlayersIds];
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
                if (await _teamService.GetProtectablePlayersAsync(TeamId) is IList<SelectableDto> players)
                {
                    IList<SelectablePlayerViewModel> wrappedPlayers = [.. players.Select(p =>
                    {
                        SelectablePlayerViewModel wrapper = new(p) { IsSelected = RosterSettingsProtectedPlayersIds.Contains(p.Id) };
                        return wrapper;
                    })];

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
            int selectedCount = ProtectablePlayers.Count(p => p.IsSelected);

            if (!playerWrapper.IsSelected && selectedCount >= RosterSettingsProtectedPerTeam)
            {
                ErrorMessage = $"You can only protect {RosterSettingsProtectedPerTeam} players";
                return;
            }

            playerWrapper.IsSelected = !playerWrapper.IsSelected;
            RosterSettingsProtectedPlayersIds = [.. ProtectablePlayers.Where(p => p.IsSelected).Select(p => p.Id)];
            ErrorMessage = string.Empty;
        }

        [RelayCommand] public void HidePlayerSelection() => ShowPlayerSelection = false;

        [RelayCommand]
        public async Task SaveRosterSettings()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                TeamInfoDto teamDto = new()
                {
                    Id = TeamId,
                    RosterSettingsCap = RosterSettingsCap,
                    RosterSettingsMaxPerTeam = RosterSettingsMaxPerTeam,
                    RosterSettingsProtectedPerTeam = RosterSettingsProtectedPerTeam,
                    RosterSettingsProtectedPlayersIds = RosterSettingsProtectedPlayersIds
                };

                if (await _teamService.UpdateRosterSettingsAsync(teamDto))
                    await BaseService.GoToMyTeamsTabAsync();
                else
                    ErrorMessage = "Failed to save roster settings";
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