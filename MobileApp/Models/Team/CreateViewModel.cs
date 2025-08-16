using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class CreateViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public CreateViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

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
                int? teamId = await _teamService.CreateTeamAsync(Location, Abbreviation, Mascot);

                if ((teamId ?? 0) > 0)
                {
                    await Shell.Current.GoToAsync($"RosterSettings?teamId={teamId}");
                }
                else
                {
                    ErrorMessage = "Failed to create team";
                }
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