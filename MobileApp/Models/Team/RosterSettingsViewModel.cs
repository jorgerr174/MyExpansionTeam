using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class RosterSettingsViewModel : TeamBaseViewModel
    {
        private readonly TeamService _teamService;

        [ObservableProperty] private TeamInfoDto? team = null;

        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private int rosterSettingsCap = 80;
        [ObservableProperty] private int rosterSettingsMaxPerTeam = 3;
        [ObservableProperty] private int rosterSettingsProtectedPerTeam = 3;
        [ObservableProperty] private List<int> protectedPlayersIds = [];

        [ObservableProperty] private bool isAutomaticProtection = true;
        [ObservableProperty] private bool isManualProtection = false;
        [ObservableProperty] private bool showPlayerSelection = false;
        [ObservableProperty] private bool isNotLoading = true;

        [ObservableProperty] private ObservableCollection<NflTeamViewModel> nflTeams = [];
        [ObservableProperty] private NflTeamViewModel? selectedNflTeam;
        [ObservableProperty] private ObservableCollection<IGrouping<string, ProtectedPlayerViewModel>> teamPlayersGrouped = [];

        public bool HasSelectedTeam => SelectedNflTeam != null;
        public bool CanSave => !IsLoading;
        public int TeamsWithSelections => NflTeams.Count(t => t.ProtectedCount > 0);
        public double SelectionProgress => TeamsWithSelections / 32.0;
        public int TotalProtectedPlayers => ProtectedPlayersIds.Count;
        public int CurrentTeamProtectedCount => SelectedNflTeam?.ProtectedCount ?? 0;

        public RosterSettingsViewModel(TeamService teamService) : base()
        {
            _teamService = teamService;
            InitializeNflTeams();
        }

        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            IsLoading = true;
            IsNotLoading = false;
            HasLoadError = false;

            try
            {
                if (await _teamService.GetTeamDetailsAsync(teamId) is TeamInfoDto team)
                {
                    Team = team;
                    TeamName = $"{team.Location} {team.Mascot}";
                    RosterSettingsCap = team.RosterSettingsCap;
                    RosterSettingsMaxPerTeam = team.RosterSettingsMaxPerTeam;
                    RosterSettingsProtectedPerTeam = team.RosterSettingsProtectedPerTeam;
                    ProtectedPlayersIds = team.RosterSettingsProtectedPlayersIds?.ToList() ?? [];

                    UpdateTeamProtectionCounts();
                }
                else
                {
                    HasLoadError = true;
                    LoadErrorMessage = "Team not found";
                }
                return;
            }
            catch (Exception ex)
            {
                HasLoadError = true;
                LoadErrorMessage = $"Failed to load team: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                IsNotLoading = true;
            }
        }

        [RelayCommand] public async Task DisplayPlayerSelection() => ShowPlayerSelection = true;
        [RelayCommand] public async Task HidePlayerSelection() => ShowPlayerSelection = false;
        [RelayCommand] public async Task GoBackToTeam() => await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = Team.Id });


        [RelayCommand]
        public async Task TogglePlayerProtection(ProtectedPlayerViewModel playerViewModel)
        {
            if (SelectedNflTeam == null) return;

            if (playerViewModel.IsProtected)
            {
                // Remove protection
                ProtectedPlayersIds.Remove(playerViewModel.Player.Id);
                playerViewModel.IsProtected = false;
                SelectedNflTeam.ProtectedCount--;
            }
            else
            {
                // Add protection (check limit)
                if (SelectedNflTeam.ProtectedCount >= RosterSettingsProtectedPerTeam)
                {
                    await Shell.Current.DisplayAlert("Limit Reached",
                        $"You can only protect {RosterSettingsProtectedPerTeam} players per team.", "OK");
                    return;
                }

                ProtectedPlayersIds.Add(playerViewModel.Player.Id);
                playerViewModel.IsProtected = true;
                SelectedNflTeam.ProtectedCount++;
            }

            OnPropertyChanged(nameof(TeamsWithSelections));
            OnPropertyChanged(nameof(SelectionProgress));
            OnPropertyChanged(nameof(TotalProtectedPlayers));
            OnPropertyChanged(nameof(CurrentTeamProtectedCount));
        }

        [RelayCommand]
        public async Task ClearTeamSelection()
        {
            if (SelectedNflTeam == null) return;

            var playersToRemove = TeamPlayersGrouped
                .SelectMany(g => g)
                .Where(p => p.IsProtected)
                .ToList();

            foreach (var player in playersToRemove)
            {
                ProtectedPlayersIds.Remove(player.Player.Id);
                player.IsProtected = false;
            }

            SelectedNflTeam.ProtectedCount = 0;

            OnPropertyChanged(nameof(TeamsWithSelections));
            OnPropertyChanged(nameof(SelectionProgress));
            OnPropertyChanged(nameof(TotalProtectedPlayers));
            OnPropertyChanged(nameof(CurrentTeamProtectedCount));
        }

        [RelayCommand]
        public async Task SaveSettings()
        {
            if (Team == null) return;

            IsLoading = true;

            try
            {
                var updateDto = new TeamInfoDto
                {
                    Id = Team.Id,
                    Location = Team.Location,
                    Mascot = Team.Mascot,
                    Abb = Team.Abb,
                    UserUsername = Team.UserUsername,
                    Date = Team.Date,
                    Complete = Team.Complete,
                    RosterSettingsCap = RosterSettingsCap,
                    RosterSettingsMaxPerTeam = RosterSettingsMaxPerTeam,
                    RosterSettingsProtectedPerTeam = RosterSettingsProtectedPerTeam,
                    RosterSettingsProtectedPlayersIds = ProtectedPlayersIds
                };

                var success = await _teamService.UpdateRosterSettingsAsync(updateDto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Roster settings saved successfully!", "OK");
                    await Shell.Current.GoToAsync($"//TeamDetails?teamId={Team.Id}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save settings. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error saving settings: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedNflTeamChanged(NflTeamViewModel? value)
        {
            if (value != null)
            {
                LoadTeamPlayers(value.Id);
            }
            OnPropertyChanged(nameof(HasSelectedTeam));
        }

        partial void OnIsAutomaticProtectionChanged(bool value)
        {
            if (value)
            {
                IsManualProtection = false;
                ShowPlayerSelection = false;
            }
        }

        partial void OnIsManualProtectionChanged(bool value)
        {
            if (value)
            {
                IsAutomaticProtection = false;
            }
        }

        private void InitializeNflTeams()
        {
            var teams = new[]
            {
                new NflTeamViewModel { Id = 1, Name = "Arizona Cardinals", Abbreviation = "ARI" },
                new NflTeamViewModel { Id = 2, Name = "Atlanta Falcons", Abbreviation = "ATL" },
                new NflTeamViewModel { Id = 3, Name = "Baltimore Ravens", Abbreviation = "BAL" },
                new NflTeamViewModel { Id = 4, Name = "Buffalo Bills", Abbreviation = "BUF" },
                new NflTeamViewModel { Id = 5, Name = "Carolina Panthers", Abbreviation = "CAR" },
                new NflTeamViewModel { Id = 6, Name = "Chicago Bears", Abbreviation = "CHI" },
                new NflTeamViewModel { Id = 7, Name = "Cincinnati Bengals", Abbreviation = "CIN" },
                new NflTeamViewModel { Id = 8, Name = "Cleveland Browns", Abbreviation = "CLE" },
                new NflTeamViewModel { Id = 9, Name = "Dallas Cowboys", Abbreviation = "DAL" },
                new NflTeamViewModel { Id = 10, Name = "Denver Broncos", Abbreviation = "DEN" },
                new NflTeamViewModel { Id = 11, Name = "Detroit Lions", Abbreviation = "DET" },
                new NflTeamViewModel { Id = 12, Name = "Green Bay Packers", Abbreviation = "GB" },
                new NflTeamViewModel { Id = 13, Name = "Houston Texans", Abbreviation = "HOU" },
                new NflTeamViewModel { Id = 14, Name = "Indianapolis Colts", Abbreviation = "IND" },
                new NflTeamViewModel { Id = 15, Name = "Jacksonville Jaguars", Abbreviation = "JAX" },
                new NflTeamViewModel { Id = 16, Name = "Kansas City Chiefs", Abbreviation = "KC" },
                new NflTeamViewModel { Id = 17, Name = "Las Vegas Raiders", Abbreviation = "LV" },
                new NflTeamViewModel { Id = 18, Name = "Los Angeles Chargers", Abbreviation = "LAC" },
                new NflTeamViewModel { Id = 19, Name = "Los Angeles Rams", Abbreviation = "LAR" },
                new NflTeamViewModel { Id = 20, Name = "Miami Dolphins", Abbreviation = "MIA" },
                new NflTeamViewModel { Id = 21, Name = "Minnesota Vikings", Abbreviation = "MIN" },
                new NflTeamViewModel { Id = 22, Name = "New England Patriots", Abbreviation = "NE" },
                new NflTeamViewModel { Id = 23, Name = "New Orleans Saints", Abbreviation = "NO" },
                new NflTeamViewModel { Id = 24, Name = "New York Giants", Abbreviation = "NYG" },
                new NflTeamViewModel { Id = 25, Name = "New York Jets", Abbreviation = "NYJ" },
                new NflTeamViewModel { Id = 26, Name = "Philadelphia Eagles", Abbreviation = "PHI" },
                new NflTeamViewModel { Id = 27, Name = "Pittsburgh Steelers", Abbreviation = "PIT" },
                new NflTeamViewModel { Id = 28, Name = "San Francisco 49ers", Abbreviation = "SF" },
                new NflTeamViewModel { Id = 29, Name = "Seattle Seahawks", Abbreviation = "SEA" },
                new NflTeamViewModel { Id = 30, Name = "Tampa Bay Buccaneers", Abbreviation = "TB" },
                new NflTeamViewModel { Id = 31, Name = "Tennessee Titans", Abbreviation = "TEN" },
                new NflTeamViewModel { Id = 32, Name = "Washington Commanders", Abbreviation = "WAS" }
            };

            NflTeams = new ObservableCollection<NflTeamViewModel>(teams);
        }

        private async void LoadTeamPlayers(int teamId)
        {
            try
            {
                var players = await _teamService.GetProtectablePlayersAsync(teamId);
                if (players != null)
                {
                    var playerViewModels = players.Select(p => new ProtectedPlayerViewModel(p)
                    {
                        IsProtected = ProtectedPlayersIds.Contains(p.Id)
                    }).ToList();

                    var grouped = playerViewModels
                        .GroupBy(p => p.Player.Position)
                        .OrderBy(g => g.Key)
                        .ToList();

                    TeamPlayersGrouped = new ObservableCollection<IGrouping<string, ProtectedPlayerViewModel>>(grouped);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load players: {ex.Message}", "OK");
            }
        }

        private void UpdateTeamProtectionCounts()
        {
            // This would need the player-team mapping from the API
            // For now, reset all counts
            foreach (var team in NflTeams)
            {
                team.ProtectedCount = 0;
            }

            OnPropertyChanged(nameof(TeamsWithSelections));
            OnPropertyChanged(nameof(SelectionProgress));
        }
    }

    public partial class NflTeamViewModel : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string DisplayName => $"{Name} ({Abbreviation})";

        [ObservableProperty] private int protectedCount = 0;
    }
}