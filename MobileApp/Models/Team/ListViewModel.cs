using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class ListViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public ListViewModel(TeamService teamService)
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
                Teams = await _teamService.GetAllTeamsAsync() ?? new List<TeamInfoDto>();
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
            await Shell.Current.GoToAsync($"TeamDetails?teamId={teamId}");
        }
    }
}