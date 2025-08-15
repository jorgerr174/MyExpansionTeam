using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class RosterViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public RosterViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        private const decimal BaseSalaryCap = 224m; // NFL salary cap in millions

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private List<FranchiseInfo> franchises = FranchiseInfo.GetAllFranchises();
        [ObservableProperty] private FranchiseInfo? selectedFranchise;
        [ObservableProperty] private IList<SelectablePlayerViewModel> availablePlayers = new List<SelectablePlayerViewModel>();
        [ObservableProperty] private IList<SelectablePlayerViewModel> rosterPlayers = new List<SelectablePlayerViewModel>();
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
                var team = await _teamService.GetTeamRosterAsync(id);
                if (team != null)
                {
                    TeamName = $"{team.Location} {team.Mascot}";

                    // Load current roster
                    var rosterPlayersList = team.Players.Select(p =>
                    {
                        var wrapper = new SelectablePlayerViewModel(new SelectableDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Position = p.Position,
                            APY = p.APY,
                            PureAPY = p.PureAPY
                        });
                        wrapper.IsSelected = true;
                        return wrapper;
                    }).ToList();

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
                var players = await _teamService.GetSelectablePlayersAsync(franchise.Id);
                if (players != null)
                {
                    var wrappedPlayers = players.Select(p =>
                    {
                        var wrapper = new SelectablePlayerViewModel(p);
                        // Check if player is already in roster
                        wrapper.IsSelected = RosterPlayers.Any(rp => rp.Id == p.Id);
                        wrapper.IsAlreadyRostered = wrapper.IsSelected;
                        return wrapper;
                    }).ToList();

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
                if (decimal.TryParse(player.Player.PureAPY, out decimal playerSalary))
                {
                    if (CurrentSalaryCap + playerSalary > BaseSalaryCap)
                    {
                        ErrorMessage = $"Cannot add player. Would exceed salary cap by ${(CurrentSalaryCap + playerSalary - BaseSalaryCap):F1}M";
                        return;
                    }
                }

                player.IsSelected = true;
                RosterPlayers = RosterPlayers.Append(player).ToList();
            }
            else
            {
                // Removing player
                player.IsSelected = false;
                RosterPlayers = RosterPlayers.Where(p => p.Id != player.Id).ToList();
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
                var teamDto = new TeamDto();
                teamDto.Id = TeamId;

                // Convert selected players to RosteredDto
                var rosteredPlayers = RosterPlayers.Select(p => new RosteredDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    APY = p.APY,
                    PureAPY = p.Player.PureAPY,
                    FranchiseId = 0 // This would need to be tracked or retrieved
                }).ToList();

                teamDto.Players = rosteredPlayers;
                teamDto.SelectedIds = RosterPlayers.Select(p => p.Id).ToList();

                bool success = await _teamService.UpdateRosterAsync(teamDto);

                if (success)
                {
                    await Shell.Current.GoToAsync("//MyTeams");
                }
                else
                {
                    ErrorMessage = "Failed to save roster";
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

        [RelayCommand]
        public void ClearRoster()
        {
            RosterPlayers = new List<SelectablePlayerViewModel>();
            UpdateSalaryCap();

            // Update available players selection state
            foreach (var player in AvailablePlayers.Where(p => !p.IsAlreadyRostered))
            {
                player.IsSelected = false;
            }
        }

        private void UpdateSalaryCap()
        {
            CurrentSalaryCap = 0;
            foreach (var player in RosterPlayers)
            {
                if (decimal.TryParse(player.Player.PureAPY, out decimal salary))
                {
                    CurrentSalaryCap += salary;
                }
            }

            SelectedPlayerCount = RosterPlayers.Count;
            OnPropertyChanged(nameof(AvailableCap));
            OnPropertyChanged(nameof(SalaryCapText));
            OnPropertyChanged(nameof(AvailableCapText));
        }
    }
}