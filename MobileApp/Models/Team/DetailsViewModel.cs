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
                await Shell.Current.GoToAsync($"TeamEdit?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task RosterSettings()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"RosterSettings?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task BuildRoster()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Roster?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ReviewRoster()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"ReviewRoster?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task SetFormation()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Formation?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ViewTrades()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"Trades?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task ViewDraftResults()
        {
            if (Team != null)
                await Shell.Current.GoToAsync($"DraftResults?teamId={Team.Id}");
        }

        [RelayCommand]
        public async Task DuplicateTeam()
        {
            try
            {
                bool confirm = await Shell.Current.DisplayAlert("Confirm Duplicate",
                    "¿Está seguro de que desea duplicar este equipo?", "Yes", "No");

                if (!confirm) return;

                IsLoading = true;
                ErrorMessage = string.Empty;

                var duplicatedTeam = await _teamService.DuplicateTeamAsync(team.Id);

                if (duplicatedTeam != null)
                {
                    // Navigate to edit view of the new duplicated team
                    await Shell.Current.GoToAsync($"Team/Edit?teamId={duplicatedTeam.Id}");
                }
                else
                {
                    ErrorMessage = "Failed to duplicate team";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Duplicate failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task DeleteTeam()
        {
            try
            {
                bool confirm = await Shell.Current.DisplayAlert("Confirm Delete",
                    "¿Está seguro de que desea eliminar este equipo?", "Yes", "No");

                if (!confirm) return;

                IsLoading = true;
                ErrorMessage = string.Empty;

                bool success = await _teamService.DeleteTeamAsync(team.Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Team deleted successfully", "OK");
                    // Navigate back to teams list
                    await Shell.Current.GoToAsync("//Home");
                }
                else
                {
                    ErrorMessage = "Failed to delete team";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Delete failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}