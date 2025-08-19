using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Home
{
    public partial class IndexViewModel : BaseViewModel
    {
        private readonly HomeService _homeService;
        private readonly AccountService _accountService;

        public IndexViewModel(HomeService homeService, AccountService accountService)
        {
            _homeService = homeService;
            _accountService = accountService;
        }

        [ObservableProperty] private bool isAuthenticated = false;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private bool isAdmin = false;
        [ObservableProperty] private IEnumerable<TeamInfoDto> teams = new List<TeamInfoDto>();

        public bool IsNotAuthenticated => !IsAuthenticated;
        public bool ShowDeleteButton => true; // Show delete for MyTeams

        [RelayCommand]
        public async Task LoadData()
        {
            if (await _accountService.IsAuthenticatedAsync())
            {
                Username = await AccountService.GetUsernameAsync() ?? "User";
                Teams = await _homeService.GetMyTeamsAsync() ?? [];

                IsAdmin = (await _accountService.GetProfileAsync())?.Role == METCore.Enums.Types.RoleEnum.Admin;
            }

            OnPropertyChanged(nameof(IsNotAuthenticated));
        }

        [RelayCommand] public async Task GoToLogIn() => await _homeService.GoToAsync(AppRoutes.LogIn, null);
        [RelayCommand] public async Task GoToAdmin() => await _homeService.GoToAsync(AppRoutes.Admin, null);
        [RelayCommand] public async Task GoToMyTeams() => await _homeService.GoToMyTeamsTabAsync();
        [RelayCommand] public async Task GoToTeamCreate() => await _homeService.GoToAsync(AppRoutes.CreateTeam, null);
        [RelayCommand] public async Task GoToTeamDetails(int teamId) => await _homeService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = teamId });
    }
}