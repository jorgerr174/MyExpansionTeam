using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class FormationViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private string currentFormationType = "offense";
        [ObservableProperty] private string selectedFormation = "";
        [ObservableProperty] private IList<SelectablePlayerViewModel> rosterPlayers = new List<SelectablePlayerViewModel>();
        [ObservableProperty] private IList<SelectablePlayerViewModel> benchPlayers = new List<SelectablePlayerViewModel>();
        [ObservableProperty] private Dictionary<string, SelectablePlayerViewModel?> starterPositions = new();
        [ObservableProperty] private List<FormationInfo> availableFormations = new();

        public List<string> FormationTypes { get; } = new() { "offense", "defense", "special" };

        [RelayCommand]
        public async Task LoadFormation(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                var team = await _teamService.GetTeamRosterAsync(id);
                if (team != null)
                {
                    TeamName = $"{team.Location} {team.Mascot}";

                    // Load roster players
                    var rosterPlayersList = team.Players.Select(p =>
                        new SelectablePlayerViewModel(new SelectableDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Position = p.Position,
                            APY = p.APY,
                            PureAPY = p.PureAPY
                        })).ToList();

                    RosterPlayers = rosterPlayersList;
                    BenchPlayers = rosterPlayersList;

                    // Load existing lineups
                    LoadExistingLineups(team);

                    // Initialize formation type
                    ChangeFormationType("offense");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load team: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ChangeFormationType(string formationType)
        {
            CurrentFormationType = formationType;
            LoadFormationsForType(formationType);

            // Reset starters when changing formation type
            StarterPositions = new Dictionary<string, SelectablePlayerViewModel?>();
            UpdateBenchPlayers();
        }

        [RelayCommand]
        public void SelectFormation(FormationInfo formation)
        {
            SelectedFormation = formation.Name;
            InitializePositions(formation);
            OnPropertyChanged(nameof(SelectedFormation));
        }

        [RelayCommand]
        public void AssignPlayerToPosition(object[] parameters)
        {
            if (parameters.Length == 2 &&
                parameters[0] is string positionId &&
                parameters[1] is SelectablePlayerViewModel player)
            {
                // Remove player from current position if assigned
                var currentPosition = StarterPositions.FirstOrDefault(kvp => kvp.Value?.Id == player.Id);
                if (!string.IsNullOrEmpty(currentPosition.Key))
                {
                    StarterPositions[currentPosition.Key] = null;
                }

                // Assign to new position
                StarterPositions[positionId] = player;

                UpdateBenchPlayers();
                OnPropertyChanged(nameof(StarterPositions));
            }
        }

        [RelayCommand]
        public void RemovePlayerFromPosition(string positionId)
        {
            if (StarterPositions.ContainsKey(positionId))
            {
                StarterPositions[positionId] = null;
                UpdateBenchPlayers();
                OnPropertyChanged(nameof(StarterPositions));
            }
        }

        [RelayCommand]
        public async Task SaveFormation()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                TeamDto teamDto = new()
                {
                    Id = TeamId
                };

                // Create lineup based on formation type
                if (CurrentFormationType == "offense")
                    teamDto.OffLineup = CreateLineupFromStarters();
                else if (CurrentFormationType == "defense")
                    teamDto.DefLineup = CreateLineupFromStarters();
                else if (CurrentFormationType == "special")
                    teamDto.SPLineup = CreateSPLineupFromStarters();

                if (await _teamService.UpdateRosterAsync(teamDto))
                    await _teamService.GoToMyTeamsTabAsync();
                else
                    ErrorMessage = "Failed to save formation";
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

        private void LoadFormationsForType(string formationType)
        {
            var formations = FormationData.GetFormationsForType(formationType);
            AvailableFormations = formations;

            if (formations.Any())
            {
                SelectFormation(formations.First());
            }
        }

        private void InitializePositions(FormationInfo formation)
        {
            StarterPositions = new Dictionary<string, SelectablePlayerViewModel?>();

            foreach (var position in formation.Positions)
            {
                StarterPositions[position.Id] = null;
            }

            UpdateBenchPlayers();
        }

        private void UpdateBenchPlayers()
        {
            var assignedPlayers = StarterPositions.Values.Where(p => p != null).ToList();
            BenchPlayers = RosterPlayers.Except(assignedPlayers).ToList();
        }

        private void LoadExistingLineups(TeamDto team)
        {
            // TODO: Load existing lineup assignments from team.OffLineup, team.DefLineup, team.SPLineup
        }

        private LineupDto CreateLineupFromStarters()
        {
            var lineup = new LineupDto();
            lineup.Formation = SelectedFormation;

            var positionIds = StarterPositions.Keys.OrderBy(k => k).ToList();

            if (positionIds.Count > 0 && StarterPositions[positionIds[0]] != null) lineup.Player1 = StarterPositions[positionIds[0]]!.Id;
            if (positionIds.Count > 1 && StarterPositions[positionIds[1]] != null) lineup.Player2 = StarterPositions[positionIds[1]]!.Id;
            if (positionIds.Count > 2 && StarterPositions[positionIds[2]] != null) lineup.Player3 = StarterPositions[positionIds[2]]!.Id;
            if (positionIds.Count > 3 && StarterPositions[positionIds[3]] != null) lineup.Player4 = StarterPositions[positionIds[3]]!.Id;
            if (positionIds.Count > 4 && StarterPositions[positionIds[4]] != null) lineup.Player5 = StarterPositions[positionIds[4]]!.Id;
            if (positionIds.Count > 5 && StarterPositions[positionIds[5]] != null) lineup.Player6 = StarterPositions[positionIds[5]]!.Id;
            if (positionIds.Count > 6 && StarterPositions[positionIds[6]] != null) lineup.Player7 = StarterPositions[positionIds[6]]!.Id;
            if (positionIds.Count > 7 && StarterPositions[positionIds[7]] != null) lineup.Player8 = StarterPositions[positionIds[7]]!.Id;
            if (positionIds.Count > 8 && StarterPositions[positionIds[8]] != null) lineup.Player9 = StarterPositions[positionIds[8]]!.Id;
            if (positionIds.Count > 9 && StarterPositions[positionIds[9]] != null) lineup.Player10 = StarterPositions[positionIds[9]]!.Id;
            if (positionIds.Count > 10 && StarterPositions[positionIds[10]] != null) lineup.Player11 = StarterPositions[positionIds[10]]!.Id;

            return lineup;
        }

        private SPLineupDto CreateSPLineupFromStarters()
        {
            var lineup = new SPLineupDto();
            lineup.Formation = SelectedFormation;

            var positionIds = StarterPositions.Keys.OrderBy(k => k).ToList();

            if (positionIds.Count > 0 && StarterPositions[positionIds[0]] != null) lineup.Player1 = StarterPositions[positionIds[0]]!.Id;
            if (positionIds.Count > 1 && StarterPositions[positionIds[1]] != null) lineup.Player2 = StarterPositions[positionIds[1]]!.Id;
            if (positionIds.Count > 2 && StarterPositions[positionIds[2]] != null) lineup.Player3 = StarterPositions[positionIds[2]]!.Id;
            if (positionIds.Count > 3 && StarterPositions[positionIds[3]] != null) lineup.Player4 = StarterPositions[positionIds[3]]!.Id;
            if (positionIds.Count > 4 && StarterPositions[positionIds[4]] != null) lineup.Player5 = StarterPositions[positionIds[4]]!.Id;

            return lineup;
        }
    }
}