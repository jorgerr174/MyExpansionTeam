using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using MobileApp.Services;

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
                Team = await _teamService.GetTeamAsync(teamId);

                if (Team is TeamDto)
                {
                    TradeHistory = await _teamService.GetTeamTradesAsync(teamId);
                    Draft = await _teamService.GetTeamDraftAsync(teamId);
                    IsOwner = Team.UserUsername == (await AccountService.GetUsernameAsync() ?? string.Empty);
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
                PositionGroup positionGroup = new();
                string prevPosition = Team.Players.First().Position;

                foreach (RosteredDto player in Team.Players)
                {
                    if (prevPosition != player.Position)
                    {
                        RosterByPosition.Add(positionGroup);
                        prevPosition = player.Position;
                        positionGroup = new() { PositionName = player.Position };
                    }
                    else positionGroup.Players.Add(player);
                }
                foreach (PositionGroup group in RosterByPosition)
                    PositionBreakdown += $"{group.PositionName} ({group.Players.Count}), ";
            }

            if (Draft is DraftDto && (Draft.Selections?.Any() ?? false))
                foreach (KeyValuePair<int, int> selection in Draft.Selections)
                    if (Draft.Prospects.FirstOrDefault(p => p.Id == selection.Value) is ProspectDto prospect)
                        DraftResults.Add(new DraftSelection() { Pick = $"Round: {selection.Key / 100}, Pick: {selection.Key / 100}", Player = prospect });

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