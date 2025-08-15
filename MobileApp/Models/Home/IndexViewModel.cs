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
            }

            OnPropertyChanged(nameof(IsNotAuthenticated));
        }

        [RelayCommand]
        public async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("Account/LogIn");
        }
    }
}