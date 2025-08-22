using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class CreateViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private string location = string.Empty;
        [ObservableProperty] private string mascot = string.Empty;
        [ObservableProperty] private string abb = string.Empty;

        [ObservableProperty] private string locationError = string.Empty;
        [ObservableProperty] private string mascotError = string.Empty;
        [ObservableProperty] private string abbError = string.Empty;

        [ObservableProperty] private bool hasLocationError = false;
        [ObservableProperty] private bool hasMascotError = false;
        [ObservableProperty] private bool hasAbbError = false;

        [ObservableProperty] private bool isNotLoading = true;

        public bool CanCreate => !IsLoading &&
                                !string.IsNullOrWhiteSpace(Location) &&
                                !string.IsNullOrWhiteSpace(Mascot) &&
                                !string.IsNullOrWhiteSpace(Abb) &&
                                Abb.Length >= 2 && Abb.Length <= 3;

        [RelayCommand]
        public async Task CreateTeam()
        {
            if (!ValidateFields())
                return;

            IsLoading = true;
            IsNotLoading = false;

            try
            {
                TeamBasicInfoDto teamDto = new(0, Location.Trim(), Abb.Trim().ToUpper(), Mascot.Trim(), string.Empty, DateTime.Now, null);

                if (await _teamService.CreateTeamAsync(teamDto) is TeamInfoDto result)
                    await BaseService.GoToAsync(AppRoutes.TeamDetails, new() { ["TeamId"] = result.Id });
                else
                    ErrorMessage = "Failed to create team. Please try again.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating team: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                IsNotLoading = true;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            ClearErrors();

            if (string.IsNullOrWhiteSpace(Location))
            {
                LocationError = "Location is required";
                HasLocationError = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Mascot))
            {
                MascotError = "Mascot is required";
                HasMascotError = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Abb))
            {
                AbbError = "Abbreviation is required";
                HasAbbError = true;
                isValid = false;
            }
            else if (Abb.Length < 2 || Abb.Length > 3)
            {
                AbbError = "Abbreviation must be 2-3 characters";
                HasAbbError = true;
                isValid = false;
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

        partial void OnLocationChanged(string value) => OnPropertyChanged(nameof(CanCreate));
        partial void OnMascotChanged(string value) => OnPropertyChanged(nameof(CanCreate));
        partial void OnAbbChanged(string value) => OnPropertyChanged(nameof(CanCreate));
    }
}