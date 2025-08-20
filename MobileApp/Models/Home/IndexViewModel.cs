using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Home
{
    public partial class IndexViewModel(HomeService homeService, TeamService teamService, AccountService accountService) : BaseViewModel
    {
        private readonly HomeService _homeService = homeService;
        private readonly TeamService _teamService = teamService;
        private readonly AccountService _accountService = accountService;

        [ObservableProperty] private bool isAuthenticated = false;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private bool isAdmin = false;
        [ObservableProperty] private IEnumerable<TeamInfoDto> teams = [];
        [ObservableProperty] private IEnumerable<TeamInfoDto> allTeams = []; // New: All teams list

        public bool IsNotAuthenticated => !IsAuthenticated;
        public bool HasNoTeams => !AllTeams.Any() && !IsLoading;

        [RelayCommand] public static async Task GoToLogIn() => await BaseService.GoToProfileTabAsync();
        [RelayCommand] public static async Task GoToCreateTeam() => await BaseService.GoToAsync(AppRoutes.CreateTeam, null);
        [RelayCommand] public static async Task GoToTeamDetails(int teamId) => await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = teamId });

        [RelayCommand]
        public async Task LoadData()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                IsAuthenticated = await BaseService.IsAuthenticatedAsync();

                if (IsAuthenticated)
                {
                    Username = await AccountService.GetUsernameAsync() ?? "User";
                    Teams = await _homeService.GetMyTeamsAsync() ?? [];
                    IsAdmin = (await _accountService.GetProfileAsync())?.Role == METCore.Enums.Types.RoleEnum.Admin;
                }

                // Load all teams for everyone (authenticated or not)
                AllTeams = await _teamService.GetAllTeamsAsync() ?? [];

                OnPropertyChanged(nameof(IsNotAuthenticated));
                OnPropertyChanged(nameof(HasNoTeams));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}