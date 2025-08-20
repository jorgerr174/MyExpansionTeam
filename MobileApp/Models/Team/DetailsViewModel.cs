using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class DetailsViewModel(TeamService teamService) : TeamBaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private TeamInfoDto? team;

        public bool ShowLoadingState => IsLoading;
        public bool ShowErrorState => HasLoadError && !IsLoading;
        public bool ShowContent => Team != null && !IsLoading && !HasLoadError;

        [RelayCommand] public async Task GoToEditTeam() => await BaseService.GoToAsync(AppRoutes.EditTeam, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToRosterSettings() => await BaseService.GoToAsync(AppRoutes.RosterSettings, new() { ["TeamId"] = Team.Id });

        [RelayCommand] public async Task GoToRoster() => await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToReviewRoster() => await BaseService.GoToAsync(AppRoutes.ReviewRoster, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToFormation() => await BaseService.GoToAsync(AppRoutes.Formation, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToTrades() => await BaseService.GoToAsync(AppRoutes.Trades, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToDraftResults() => await BaseService.GoToAsync(AppRoutes.DraftResults, new() { ["TeamId"] = Team.Id });


        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            IsLoading = true;
            HasLoadError = false;
            LoadErrorMessage = string.Empty;
            Team = null;

            try
            {
                Team = await _teamService.GetTeamDetailsAsync(teamId);

                if (Team == null)
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
                IsLoading = false;

                // Notify UI state changes
                OnPropertyChanged(nameof(ShowLoadingState));
                OnPropertyChanged(nameof(ShowErrorState));
                OnPropertyChanged(nameof(ShowContent));
            }
        }

        [RelayCommand]
        public async Task DuplicateTeam()
        {
            if (Team != null)
            {
                IsLoading = true;
                try
                {
                    if (await _teamService.DuplicateTeamAsync(Team.Id) is TeamBasicInfoDto newTeam)
                        await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = Team.Id });
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
}