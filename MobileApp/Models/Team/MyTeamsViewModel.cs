using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class MyTeamsViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public MyTeamsViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        [ObservableProperty] private IEnumerable<TeamInfoDto> teams = new List<TeamInfoDto>();


        [RelayCommand]
        public async Task LoadTeams()
        {
            IsLoading = true;
            try
            {
                Teams = await _teamService.GetMyTeamsAsync() ?? new List<TeamInfoDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load teams: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ViewTeam(int teamId)
        {
            await Shell.Current.GoToAsync($"Team/Details?teamId={teamId}");
        }

        [RelayCommand]
        public async Task DeleteTeam(int teamId)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete",
                "Are you sure you want to delete this team?",
                "Yes", "No");

            if (!confirm) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                bool success = await _teamService.DeleteTeamAsync(teamId);
                if (success)
                {
                    // Reload the teams list to reflect the deletion
                    await LoadTeams();
                }
                else
                {
                    ErrorMessage = "Failed to delete team";
                }
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