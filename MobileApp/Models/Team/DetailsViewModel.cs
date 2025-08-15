using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class DetailsViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;
        private readonly AccountService _accountService;

        public DetailsViewModel(TeamService teamService, AccountService accountService)
        {
            _teamService = teamService;
            _accountService = accountService;
        }


        [ObservableProperty] private TeamInfoDto? team;
        [ObservableProperty] private bool isOwner = false;

        [RelayCommand]
        public async Task LoadTeam(int teamId)
        {
            IsLoading = true;
            try
            {
                Team = await _teamService.GetTeamDetailsAsync(teamId);
                if (Team != null)
                {
                    var currentUser = await AccountService.GetUsernameAsync();
                    IsOwner = Team.UserUsername == currentUser;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load team: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task EditTeam()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/Edit?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task RosterSettings()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/RosterSettings?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task BuildRoster()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/Roster?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ReviewRoster()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/ReviewRoster?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task SetFormation()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/Formation?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ViewTrades()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/Trades?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ViewDraftResults()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Team/DraftResults?teamId={Team.Id}");
        }
    }
}