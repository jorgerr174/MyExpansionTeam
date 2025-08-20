using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class CreateViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        [ObservableProperty] private string location = string.Empty;
        [ObservableProperty] private string abbreviation = string.Empty;
        [ObservableProperty] private string mascot = string.Empty;

        [RelayCommand]
        public async Task CreateTeam()
        {
            if (string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(Abbreviation) || string.IsNullOrWhiteSpace(Mascot))
            {
                ErrorMessage = "Location, Abbreviation, and Mascot are required";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                if (await _teamService.CreateTeamAsync(Location, Abbreviation, Mascot) is int teamId && teamId > 0)
                    await _teamService.GoToAsync(AppRoutes.RosterSettings, new() { ["TeamId"] = teamId });
                else
                    ErrorMessage = "Failed to create team";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Create failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}