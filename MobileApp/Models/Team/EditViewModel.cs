using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class EditViewModel(TeamService teamService) : TeamBaseViewModel()
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private TeamInfoDto team = null;

        [ObservableProperty] private string location = string.Empty;
        [ObservableProperty] private string mascot = string.Empty;
        [ObservableProperty] private string abb = string.Empty;

        [ObservableProperty] private string locationError = string.Empty;
        [ObservableProperty] private string mascotError = string.Empty;
        [ObservableProperty] private string abbError = string.Empty;

        [ObservableProperty] private bool hasLocationError = false;
        [ObservableProperty] private bool hasMascotError = false;
        [ObservableProperty] private bool hasAbbError = false;

        [ObservableProperty] private bool isSaving = false;
        [ObservableProperty] private bool isDuplicating = false;
        [ObservableProperty] private bool isDeleting = false;

        public bool CanSave => !IsSaving && !IsLoading;
        public bool CanDuplicate => !IsDuplicating && !IsLoading;
        public bool CanDelete => !IsDeleting && !IsLoading;



        [RelayCommand] public async Task Cancel() => await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = Team.Id });
        [RelayCommand] public async Task GoToEditSettings() => await BaseService.GoToAsync(AppRoutes.RosterSettings, new() { ["TeamId"] = Team.Id });


        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            IsLoading = true;
            HasLoadError = false;
            LoadErrorMessage = string.Empty;

            try
            {
                if (await _teamService.GetTeamDetailsAsync(teamId) is TeamInfoDto team)
                {
                    Team = team;
                    ClearErrors();
                }
                else
                {
                    HasLoadError = true;
                    LoadErrorMessage = "Team not found";
                }
            }
            catch (Exception ex)
            {
                HasLoadError = true;
                LoadErrorMessage = $"Failed to load team: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (!ValidateFields())
                return;

            IsSaving = true;

            try
            {
                if (await _teamService.EditAsync(new(Team.Id, GetEffectiveValue(Location, Team.Location), GetEffectiveValue(Abb, Team.Abb).ToUpper(),
                    GetEffectiveValue(Mascot, Team.Mascot), Team.UserUsername, Team.Date, false)))
                {
                    await LoadViewAsync(Team.Id);
                    await Shell.Current.DisplayAlert("Success", "Team updated successfully!", "OK");
                }
                else
                    await Shell.Current.DisplayAlert("Error", "Failed to update team. Please try again.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error updating team: {ex.Message}", "OK");
            }
            finally
            {
                IsSaving = false;
                OnPropertyChanged(nameof(CanSave));
            }
        }

        [RelayCommand]
        public async Task Duplicate()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Duplicate Team",
                $"Are you sure you want to duplicate {Team.Location} {Team.Mascot}?",
                "Yes", "No");

            if (!confirm) return;

            IsDuplicating = true;

            try
            {
                if (await _teamService.DuplicateTeamAsync(Team.Id) is ResultDto<TeamBasicInfoDto> result
                    && String.IsNullOrWhiteSpace(result.Message) && result.Value is TeamBasicInfoDto newTeam)
                    await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = newTeam.Id });
                else
                    await Shell.Current.DisplayAlert("Error", "Failed to duplicate team. Please try again.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error duplicating team: {ex.Message}", "OK");
            }
            finally
            {
                IsDuplicating = false;
                OnPropertyChanged(nameof(CanDuplicate));
            }
        }

        [RelayCommand]
        public async Task Delete()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Delete Team",
                $"Are you sure you want to delete {Team.Location} {Team.Mascot}? This action cannot be undone.",
                "Delete", "Cancel");

            if (!confirm) return;

            IsDeleting = true;

            try
            {
                var success = await _teamService.DeleteTeamAsync(Team.Id);

                if (success)
                {
                    // Navigate back to MyTeams
                    await Shell.Current.GoToAsync("//MyTeamsTab");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete team. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error deleting team: {ex.Message}", "OK");
            }
            finally
            {
                IsDeleting = false;
                OnPropertyChanged(nameof(CanDelete));
            }
        }

        [RelayCommand]
        public async Task EditRoster()
        {
            await Shell.Current.GoToAsync($"//Roster?teamId={Team.Id}");
        }

        private bool ValidateFields()
        {
            bool isValid = true;
            ClearErrors();

            if (!string.IsNullOrWhiteSpace(Location) && string.IsNullOrWhiteSpace(Location.Trim()))
            {
                LocationError = "Location cannot be empty";
                HasLocationError = true;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(Mascot) && string.IsNullOrWhiteSpace(Mascot.Trim()))
            {
                MascotError = "Mascot cannot be empty";
                HasMascotError = true;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(Abb))
            {
                var trimmedAbb = Abb.Trim();
                if (string.IsNullOrWhiteSpace(trimmedAbb))
                {
                    AbbError = "Abbreviation cannot be empty";
                    HasAbbError = true;
                    isValid = false;
                }
                else if (trimmedAbb.Length < 2 || trimmedAbb.Length > 3)
                {
                    AbbError = "Abbreviation must be 2-3 characters";
                    HasAbbError = true;
                    isValid = false;
                }
            }

            return isValid;
        }

        private void ClearErrors()
        {
            LocationError = string.Empty;
            MascotError = string.Empty;
            AbbError = string.Empty;
            HasLocationError = false;
            HasMascotError = false;
            HasAbbError = false;
        }

        private static string GetEffectiveValue(string currentValue, string originalValue) => string.IsNullOrWhiteSpace(currentValue) ? originalValue : currentValue.Trim();

        partial void OnLocationChanged(string value) => ValidateFields();
        partial void OnMascotChanged(string value) => ValidateFields();
        partial void OnAbbChanged(string value) => ValidateFields();
    }
}