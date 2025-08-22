using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class MyTeamsViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private IEnumerable<TeamInfoDto> teams = [];

        public bool HasTeams => !IsLoading && Teams.Any();
        public bool HasNoTeams => !IsLoading && !Teams.Any();


        [RelayCommand] public static async Task GoToCreateTeam() => await BaseService.GoToAsync(AppRoutes.CreateTeam, null);
        [RelayCommand] public static async Task GoToTeamDetails(int teamId) => await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = teamId });
        [RelayCommand] public static async Task GoToEditTeam(int teamId) => await BaseService.GoToAsync(AppRoutes.EditTeam, new() { ["TeamId"] = teamId });


        [RelayCommand]
        public async Task LoadTeams()
        {
            IsLoading = true;
            try
            {
                Teams = await _teamService.GetMyTeamsAsync() ?? [];
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load teams: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
            OnPropertyChanged(nameof(HasTeams));
            OnPropertyChanged(nameof(HasNoTeams));
        }
    }
}