using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class EditViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public EditViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string location = string.Empty;
        [ObservableProperty] private string abbreviation = string.Empty;
        [ObservableProperty] private string mascot = string.Empty;

        [RelayCommand]
        public async Task LoadTeam(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                var team = await _teamService.GetTeamDetailsAsync(id);
                if (team != null)
                {
                    Location = team.Location;
                    Abbreviation = team.Abb;
                    Mascot = team.Mascot;
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
        public async Task UpdateTeam()
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
                bool success = await _teamService.UpdateTeamAsync(TeamId, Location, Abbreviation, Mascot);

                if (success)
                {
                    await Shell.Current.GoToAsync("MyTeams");
                }
                else
                {
                    ErrorMessage = "Failed to update team";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Update failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
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

                var duplicatedTeam = await _teamService.DuplicateTeamAsync(TeamId);

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

                bool success = await _teamService.DeleteTeamAsync(TeamId);

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