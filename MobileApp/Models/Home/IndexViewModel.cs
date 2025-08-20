using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Home
{
    public partial class IndexViewModel(HomeService homeService, AccountService accountService) : BaseViewModel
    {
        private readonly HomeService _homeService = homeService;
        private readonly AccountService _accountService = accountService;

        [ObservableProperty] private bool isAuthenticated = false;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private bool isAdmin = false;
        [ObservableProperty] private IEnumerable<TeamInfoDto> teams = [];

        public bool IsNotAuthenticated => !IsAuthenticated;
        public static bool ShowDeleteButton => true; // Show delete for MyTeams


        [RelayCommand] public static async Task GoToMyTeams() => await BaseService.GoToMyTeamsTabAsync();
        [RelayCommand] public static async Task GoToLogIn() => await BaseService.GoToAsync(AppRoutes.LogIn, null);
        [RelayCommand] public static async Task GoToAdmin() => await BaseService.GoToAsync(AppRoutes.Admin, null);
        [RelayCommand] public static async Task GoToCreateTeam() => await BaseService.GoToAsync(AppRoutes.CreateTeam, null);
        [RelayCommand] public static async Task GoToTeamDetails(int teamId) => await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = teamId });


        [RelayCommand]
        public async Task LoadData()
        {
            if (await BaseService.IsAuthenticatedAsync())
            {
                Username = await AccountService.GetUsernameAsync() ?? "User";
                Teams = await _homeService.GetMyTeamsAsync() ?? [];

                IsAdmin = (await _accountService.GetProfileAsync())?.Role == METCore.Enums.Types.RoleEnum.Admin;
            }

            OnPropertyChanged(nameof(IsNotAuthenticated));
        }
    }
}