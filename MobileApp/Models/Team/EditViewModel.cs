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

        [ObservableProperty] private string originalLocation = string.Empty;
        [ObservableProperty] private string originalMascot = string.Empty;
        [ObservableProperty] private string originalAbb = string.Empty;
        public bool HasTeam => Team is TeamInfoDto;

        public bool CanAction => !IsDeleting && !IsDuplicating && !IsSaving && !IsLoading;



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
                    OriginalLocation = team.Location;
                    OriginalMascot = team.Mascot;
                    OriginalAbb = team.Abb;
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
            OnPropertyChanged(nameof(HasTeam));
            OnPropertyChanged(nameof(CanAction));
        }

        [RelayCommand]
        public async Task Save()
        {
            if (!ValidateFields())
                return;

            IsSaving = true;
            OnPropertyChanged(nameof(CanAction));

            try
            {
                if (await _teamService.EditAsync(new(Team.Id, GetEffectiveValue(Location, Team.Location), GetEffectiveValue(Abb, Team.Abb).ToUpper(),
                    GetEffectiveValue(Mascot, Team.Mascot), Team.UserUsername, Team.Date, false)))
                {
                    await LoadViewAsync(Team.Id);
                    await Shell.Current.DisplayAlert("Success", "!Equipo guardado con éxito!", "OK");
                }
                else
                    await Shell.Current.DisplayAlert("Error", "Error al guardar el equipo. Por favor, pruebe de nuevo.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error guardando el equipo: {ex.Message}", "OK");
            }
            finally
            {
                IsSaving = false;
                OnPropertyChanged(nameof(CanAction));
            }
        }

        [RelayCommand]
        public async Task Duplicate()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Duplicar Equipo",
                $"¿Estás seguro que quieres duplicar {Team.Location} {Team.Mascot}?",
                "Sí", "No");

            if (!confirm) return;
            IsDuplicating = true;
            OnPropertyChanged(nameof(CanAction));

            try
            {
                if (await _teamService.DuplicateTeamAsync(Team.Id) is ResultDto<TeamBasicInfoDto> result
                    && String.IsNullOrWhiteSpace(result.Message) && result.Value is TeamBasicInfoDto newTeam)
                    await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = newTeam.Id });
                else
                    await Shell.Current.DisplayAlert("Error", "Duplicado fallido. Por favor, pruebe de nuevo.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error durante el duplicado: {ex.Message}", "OK");
            }
            finally
            {
                IsDuplicating = false;
                OnPropertyChanged(nameof(CanAction));
            }
        }

        [RelayCommand]
        public async Task Delete()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Borrado de Equipo",
                $"¿Estás seguro que quieres borrar {Team.Location} {Team.Mascot}? Esta acción no se podrá deshacer.",
                "Borrar", "Cancel");

            if (!confirm) return;
            IsDeleting = true;
            OnPropertyChanged(nameof(CanAction));

            try
            {
                if (await _teamService.DeleteTeamAsync(Team.Id)) BaseService.GoToMyTeamsTabAsync();
                else await Shell.Current.DisplayAlert("Error", "Error al borrar el equipo. Por favor, pruebe de nuevo.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error borrando el equipo: {ex.Message}", "OK");
            }
            finally
            {
                IsDeleting = false;
                OnPropertyChanged(nameof(CanAction));
            }
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