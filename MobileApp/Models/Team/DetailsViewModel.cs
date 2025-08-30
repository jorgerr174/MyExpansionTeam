using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Team
{
    public partial class DetailsViewModel(TeamService teamService) : TeamBaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private TeamDto? team = null;

        [ObservableProperty] private IList<PositionGroup> rosterByPosition = [];
        [ObservableProperty] private decimal totalCap = 0;
        [ObservableProperty] private string positionBreakdown = string.Empty;

        [ObservableProperty] private IList<TradeDto>? tradeHistory = [];
        [ObservableProperty] private IList<DraftSelection>? draftResults = [];
        [ObservableProperty] private DraftDto? draft = null;

        [ObservableProperty] private bool hasTeam = false;
        [ObservableProperty] private bool isOwner = false;
        [ObservableProperty] private bool showLoadingState = false;
        [ObservableProperty] private bool showErrorState = false;
        [ObservableProperty] private bool showContent = false;

        // Formation Display Properties
        [ObservableProperty] private string viewedFormationType = "offense";
        [ObservableProperty] private Color offenseViewColor = Color.FromArgb("#007bff");
        [ObservableProperty] private Color offenseViewTextColor = Colors.White;
        [ObservableProperty] private Color defenseViewColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color defenseViewTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color specialViewColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color specialViewTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private string currentFormationDisplayName = "";
        // Visibility Properties
        [ObservableProperty] private bool isOffenseViewSelected = true;
        [ObservableProperty] private bool isDefenseViewSelected = false;
        [ObservableProperty] private bool isSpecialViewSelected = false;

        public Thickness FDylMargin => new(IsDefenseViewSelected ? 90 : 10);
        public LayoutOptions SylVo => new(IsDefenseViewSelected ? LayoutAlignment.End : LayoutAlignment.Start, true);
        public Thickness SylMargin => new(IsDefenseViewSelected ? 60 : 95);


        // Pre-generated Formation Collections (Load Once)
        public ObservableCollection<FormationDisplayPosition> OffensePositions { get; } = [];
        public ObservableCollection<FormationDisplayPosition> DefensePositions { get; } = [];
        public ObservableCollection<FormationDisplayPosition> SpecialPositions { get; } = [];
        private string _offenseFormationKey = "";
        private string _defenseFormationKey = "";
        private string _specialFormationKey = "";

        public ObservableCollection<FormationDisplayPosition> CurrentFormationPositions { get; } = [];

        [RelayCommand] public async Task GoToEditTeam() => await BaseService.GoToAsync(AppRoutes.EditTeam, new() { ["TeamId"] = Team.Id });

        [RelayCommand] public async Task GoToRosterSettings() => await BaseService.GoToAsync(AppRoutes.RosterSettings, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToRoster() => await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToFormation() => await BaseService.GoToAsync(AppRoutes.Formation, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToTrade() => await BaseService.GoToAsync(AppRoutes.Trade, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToDraft() => await BaseService.GoToAsync(AppRoutes.Draft, new() { ["TeamId"] = Team.Id });


        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            UpdateLoadingState(true);

            try
            {
                var teamTask = _teamService.GetTeamAsync(teamId);
                var tradesTask = _teamService.GetTeamTradesAsync(teamId);
                var draftTask = _teamService.GetTeamDraftAsync(teamId);

                Team = await teamTask;

                if (Team is not null)
                {
                    TradeHistory = await tradesTask;
                    Draft = await draftTask;
                    IsOwner = Team.UserUsername == (await AccountService.GetUsernameAsync() ?? string.Empty);

                    LoadAllFormationDisplays();
                    UpdateFormationViewButtons();
                    HasLoadError = false;
                    LoadErrorMessage = string.Empty;
                }
                else
                {
                    HasLoadError = true;
                    LoadErrorMessage = "Team not found";
                }
            }
            catch (Exception ex)
            {
                HasLoadError = true;
                LoadErrorMessage = $"Failed to load team: {ex.Message}";
            }
            finally
            {
                UpdateLoadingState(false);
                UpdateContentState();
            }
        }

        private void UpdateLoadingState(bool loading)
        {
            IsLoading = loading;
            ShowLoadingState = loading;
        }

        private void UpdateContentState()
        {
            HasTeam = !IsLoading && !HasLoadError && Team != null;
            TotalCap = HasTeam ? Team.Players.Sum(p => decimal.Parse(p.PureAPY.Replace('.', ','))) : 0;

            if (Team?.Players?.Any() ?? false)
            {
                string prevPosition = Team.Players.First().Position;
                PositionGroup positionGroup = new() { PositionName = prevPosition };

                foreach (RosteredDto player in Team.Players)
                {
                    if (prevPosition != player.Position)
                    {
                        RosterByPosition.Add(positionGroup);
                        prevPosition = player.Position;
                        positionGroup = new() { PositionName = prevPosition };
                    }
                    positionGroup.Players.Add(player);
                }
                RosterByPosition.Add(positionGroup);

                PositionBreakdown = string.Empty;
                foreach (PositionGroup group in RosterByPosition)
                    PositionBreakdown += $"{group.PositionName} ({group.Players.Count}), ";
            }

            if (Draft is DraftDto && (Draft.Selections?.Any() ?? false))
                foreach (KeyValuePair<int, int> selection in Draft.Selections)
                    if (Draft.Prospects.FirstOrDefault(p => p.Id == selection.Value) is ProspectDto prospect)
                    {
                        (int round, int pos) = DraftPicks.GetPickRoundPosFromOverall(selection.Key);
                        DraftResults.Add(new DraftSelection() { Pick = $"Round: {round}, Pick: {pos}", Player = prospect });
                    }

            ShowErrorState = HasLoadError && !IsLoading;
            ShowContent = HasTeam;
        }

        [RelayCommand]
        public async Task DuplicateTeam()
        {
            if (Team != null)
            {
                IsLoading = true;
                try
                {
                    if (await _teamService.DuplicateTeamAsync(Team.Id) is ResultDto<TeamBasicInfoDto> result
                        && String.IsNullOrWhiteSpace(result.Message) && result.Value is TeamBasicInfoDto newTeam)
                        await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = newTeam.Id });
                    else
                        ErrorMessage = "Failed to duplicate team";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Duplicate failed: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        [RelayCommand]
        public async Task DeleteTeam()
        {
            if (Team != null)
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Delete Team",
                    $"Are you sure you want to delete {Team.Location} {Team.Mascot}?",
                    "Yes", "No");

                if (confirm)
                {
                    IsLoading = true;
                    try
                    {
                        if (await _teamService.DeleteTeamAsync(Team.Id))
                            await BaseService.GoToMyTeamsTabAsync();
                        else
                            ErrorMessage = "Failed to delete team";
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Delete failed: {ex.Message}";
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                }
            }
        }

        [RelayCommand]
        public void ViewFormation(string formationType)
        {
            ViewedFormationType = formationType;
            UpdateFormationViewButtons();
            UpdateCurrentFormationName();
        }

        private void UpdateFormationViewButtons()
        {
            // Reset all
            OffenseViewColor = Color.FromArgb("#f8f9fa");
            OffenseViewTextColor = Color.FromArgb("#6c757d");
            DefenseViewColor = Color.FromArgb("#f8f9fa");
            DefenseViewTextColor = Color.FromArgb("#6c757d");
            SpecialViewColor = Color.FromArgb("#f8f9fa");
            SpecialViewTextColor = Color.FromArgb("#6c757d");

            IsOffenseViewSelected = false;
            IsDefenseViewSelected = false;
            IsSpecialViewSelected = false;

            // Set active
            switch (ViewedFormationType)
            {
                case "offense":
                    OffenseViewColor = Color.FromArgb("#007bff");
                    OffenseViewTextColor = Colors.White;
                    IsOffenseViewSelected = true;
                    break;
                case "defense":
                    DefenseViewColor = Color.FromArgb("#007bff");
                    DefenseViewTextColor = Colors.White;
                    IsDefenseViewSelected = true;
                    break;
                case "special":
                    SpecialViewColor = Color.FromArgb("#007bff");
                    SpecialViewTextColor = Colors.White;
                    IsSpecialViewSelected = true;
                    break;
            }
            OnPropertyChanged(nameof(FDylMargin));
            OnPropertyChanged(nameof(SylVo));
            OnPropertyChanged(nameof(SylMargin));
        }

        private void UpdateCurrentFormationName()
        {
            string formationKey = string.Empty;
            IList<FormationInfo> formations = [];

            switch (ViewedFormationType)
            {
                case "offense":
                    formationKey = _offenseFormationKey;
                    formations = FormationData.GetOffenseFormations();
                    break;
                case "defense":
                    formationKey = _defenseFormationKey;
                    formations = FormationData.GetDefenseFormations();
                    break;
                case "special":
                    formationKey = _specialFormationKey;
                    formations = FormationData.GetSpecialTeamsFormations();
                    break;
                default: break;
            }

            CurrentFormationDisplayName =
                formations.FirstOrDefault(f => f.Key == formationKey) is FormationInfo formation
                ? formation.Name
                : "Unknown Formation";
        }

        private void LoadAllFormationDisplays()
        {
            _offenseFormationKey = !String.IsNullOrWhiteSpace(Team?.OffLineup?.Formation) ? Team.OffLineup.Formation : "Eleven";
            _defenseFormationKey = !String.IsNullOrWhiteSpace(Team?.DefLineup?.Formation) ? Team.DefLineup.Formation : "FourThree";
            _specialFormationKey = !String.IsNullOrWhiteSpace(Team?.SPLineup?.Formation) ? Team.SPLineup.Formation : "SpecialTeams";

            GenerateFormationDisplay("offense", _offenseFormationKey, FormationData.GetOffenseFormations(), OffensePositions, Team?.OffLineup);
            GenerateFormationDisplay("defense", _defenseFormationKey, FormationData.GetDefenseFormations(), DefensePositions, Team?.DefLineup);
            GenerateFormationDisplay("special", _specialFormationKey, FormationData.GetSpecialTeamsFormations(), SpecialPositions, Team?.SPLineup);

            UpdateCurrentFormationName();
        }

        private void GenerateFormationDisplay(string formationType, string formationKey, IList<FormationInfo> formations,
            ObservableCollection<FormationDisplayPosition> collection, SPLineupDto? lineup)
        {
            collection.Clear();
            if (formations.FirstOrDefault(f => f.Key == formationKey) is FormationInfo formation)
                for (int i = 0; i < formation.Positions.Count; i++)
                {
                    var pos = formation.Positions[i];
                    var player = Team?.Players?.FirstOrDefault(p => p.Id == GetPlayerIdFromLineup(lineup, i + 1));

                    collection.Add(new FormationDisplayPosition(pos.Name, player?.Name ?? "Empty", pos.X, pos.Y, player != null));
                }
        }

        private int GetPlayerIdFromLineup(SPLineupDto? splineup, int position)
        {
            if (splineup == null) return 0;

            return splineup is LineupDto lineup
                ? position switch
                {
                    1 => lineup.Player1,
                    2 => lineup.Player2,
                    3 => lineup.Player3,
                    4 => lineup.Player4,
                    5 => lineup.Player5,
                    6 => lineup.Player6,
                    7 => lineup.Player7,
                    8 => lineup.Player8,
                    9 => lineup.Player9,
                    10 => lineup.Player10,
                    11 => lineup.Player11,
                    _ => 0
                }
                : position switch
                {
                    1 => splineup.Player1,
                    2 => splineup.Player2,
                    3 => splineup.Player3,
                    4 => splineup.Player4,
                    5 => splineup.Player5,
                    _ => 0
                };
        }
    }

    public class PositionGroup
    {
        public string PositionName { get; set; } = string.Empty;
        public IList<RosteredDto> Players { get; set; } = [];
    }

    public class DraftSelection
    {
        public string Pick { get; set; } = string.Empty;
        public ProspectDto Player { get; set; } = null;
    }
}