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
            IsAuthenticated = await _accountService.IsAuthenticatedAsync();

            if (IsAuthenticated)
            {
                Username = await AccountService.GetUsernameAsync() ?? "User";
                Teams = await _homeService.GetMyTeamsAsync() ?? [];

                IsAdmin = (await _accountService.GetProfileAsync())?.Role == METCore.Enums.Types.RoleEnum.Admin;
            }

            OnPropertyChanged(nameof(IsNotAuthenticated));
        }

        [RelayCommand]
        public async Task GoToLogIn()
        {
            await Shell.Current.GoToAsync("LogIn");
        }

        [RelayCommand]
        public async Task GoToAdmin()
        {
            await Shell.Current.GoToAsync("Admin");
        }

        [RelayCommand]
        public async Task MyTeams()
        {
            await Shell.Current.GoToAsync("MyTeams");
        }

        [RelayCommand]
        public async Task CreateTeam()
        {
            await Shell.Current.GoToAsync("TeamCreate");
        }

        [RelayCommand]
        public async Task ViewTeam(int teamId)
        {
            await Shell.Current.GoToAsync($"TeamDetails?teamId={teamId}");
        }
    }
}