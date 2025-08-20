using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class RosterViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        private const decimal BaseSalaryCap = 224m; // NFL salary cap in millions

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private List<FranchiseInfo> franchises = FranchiseInfo.GetAllFranchises();
        [ObservableProperty] private FranchiseInfo? selectedFranchise;
        [ObservableProperty] private IList<SelectablePlayerViewModel> availablePlayers = [];
        [ObservableProperty] private IList<SelectablePlayerViewModel> rosterPlayers = [];
        [ObservableProperty] private bool showPlayerSelection = false;
        [ObservableProperty] private decimal currentSalaryCap = 0m;
        [ObservableProperty] private int selectedPlayerCount = 0;

        // Salary cap properties
        public decimal AvailableCap => BaseSalaryCap - CurrentSalaryCap;
        public string SalaryCapText => $"Cap: ${CurrentSalaryCap:F1}M / ${BaseSalaryCap}M";
        public string AvailableCapText => $"Available: ${AvailableCap:F1}M";

        [RelayCommand]
        public async Task LoadRoster(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                if (await _teamService.GetTeamRosterAsync(id) is TeamDto team)
                {
                    TeamName = $"{team.Location} {team.Mascot}";

                    // Load current roster
                    IList<SelectablePlayerViewModel> rosterPlayersList = [.. team.Players.Select(p =>
                    {
                        SelectablePlayerViewModel wrapper = new(new SelectableDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Position = p.Position,
                            APY = p.APY,
                            PureAPY = p.PureAPY
                        }) { IsSelected = true };
                        return wrapper;
                    })];

                    RosterPlayers = rosterPlayersList;
                    UpdateSalaryCap();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load roster: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SelectFranchise(FranchiseInfo franchise)
        {
            SelectedFranchise = franchise;
            IsLoading = true;

            try
            {
                if (await _teamService.GetSelectablePlayersAsync(franchise.Id) is IList<SelectableDto> players)
                {
                    IList<SelectablePlayerViewModel> wrappedPlayers = [.. players.Select(p =>
                    {
                        SelectablePlayerViewModel wrapper = new(p) {
                            // Check if player is already in roster
                        IsSelected = RosterPlayers.Any(rp => rp.Id == p.Id) };
                        wrapper.IsAlreadyRostered = wrapper.IsSelected;
                        return wrapper;
                    })];

                    AvailablePlayers = wrappedPlayers;
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
        public void TogglePlayerSelection(SelectablePlayerViewModel player)
        {
            if (player.IsAlreadyRostered && player.IsSelected)
            {
                ErrorMessage = "Player is already in your roster";
                return;
            }

            if (!player.IsSelected)
            {
                // Adding player - check salary cap
                if (decimal.TryParse(player.Player.PureAPY, out decimal playerSalary)
                    && (CurrentSalaryCap + playerSalary) > BaseSalaryCap)
                {
                    ErrorMessage = $"Cannot add player. Would exceed salary cap by ${(CurrentSalaryCap + playerSalary - BaseSalaryCap):F1}M";
                    return;
                }

                player.IsSelected = true;
                RosterPlayers = [.. RosterPlayers, player];
            }
            else
            {
                // Removing player
                player.IsSelected = false;
                RosterPlayers = [.. RosterPlayers.Where(p => p.Id != player.Id)];
            }

            UpdateSalaryCap();
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        public void HidePlayerSelection()
        {
            ShowPlayerSelection = false;
            SelectedFranchise = null;
        }

        [RelayCommand]
        public async Task SaveRoster()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                TeamDto teamDto = new() { Id = TeamId };

                IList<RosteredDto> rosteredPlayers = [.. RosterPlayers.Select(p => new RosteredDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    APY = p.APY,
                    PureAPY = p.Player.PureAPY,
                    FranchiseId = 0
                })];

                teamDto.Players = rosteredPlayers;
                teamDto.SelectedIds = [.. RosterPlayers.Select(p => p.Id)];

                if (await _teamService.UpdateRosterAsync(teamDto))
                    await _teamService.GoToMyTeamsTabAsync();
                else
                    ErrorMessage = "Failed to save roster";
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

        [RelayCommand]
        public void ClearRoster()
        {
            RosterPlayers = [];
            UpdateSalaryCap();

            // Update available players selection state
            foreach (var player in AvailablePlayers.Where(p => !p.IsAlreadyRostered))
                player.IsSelected = false;
        }

        private void UpdateSalaryCap()
        {
            CurrentSalaryCap = 0;
            foreach (var player in RosterPlayers)
                if (decimal.TryParse(player.Player.PureAPY, out decimal salary))
                    CurrentSalaryCap += salary;

            SelectedPlayerCount = RosterPlayers.Count;
            OnPropertyChanged(nameof(AvailableCap));
            OnPropertyChanged(nameof(SalaryCapText));
            OnPropertyChanged(nameof(AvailableCapText));
        }
    }
}