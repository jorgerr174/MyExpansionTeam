using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class ReviewRosterViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        private const decimal BaseSalaryCap = 224m;

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private IList<SelectablePlayerViewModel> allRosterPlayers = [];
        [ObservableProperty] private IList<SelectablePlayerViewModel> filteredPlayers = [];
        [ObservableProperty] private string selectedPositionFilter = "All";
        [ObservableProperty] private decimal currentSalaryCap = 0m;

        public List<string> PositionFilters { get; } =
        [
            "All", "QB", "RB", "WR", "TE", "OL", "DL", "LB", "DB", "K", "P"
        ];

        public decimal AvailableCap => BaseSalaryCap - CurrentSalaryCap;
        public string SalaryCapText => $"Límite salarial: ${CurrentSalaryCap:F1}M / ${BaseSalaryCap}M";
        public string AvailableCapText => $"Disponible: ${AvailableCap:F1}M";
        public int TotalPlayersCount => AllRosterPlayers.Count;


        [RelayCommand] public async Task GoToRoster() => await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = TeamId });


        [RelayCommand]
        public async Task LoadRoster(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                if (await _teamService.GetTeamAsync(id) is TeamDto team)
                {
                    TeamName = $"{team.Location} {team.Mascot}";

                    IList<SelectablePlayerViewModel> rosterPlayersList = [.. team.Players.Select(p =>
                    {
                        SelectablePlayerViewModel wrapper = new(new SelectableDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Position = p.Position,
                            APY = p.APY,
                            PureAPY = p.PureAPY
                        }) { IsSelected = true };
                        return wrapper;
                    })];

                    AllRosterPlayers = rosterPlayersList;
                    ApplyPositionFilter();
                    UpdateSalaryCap();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar la plantilla: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void FilterByPosition(string position)
        {
            SelectedPositionFilter = position;
            ApplyPositionFilter();
        }

        [RelayCommand]
        public void RemovePlayer(SelectablePlayerViewModel player)
        {
            AllRosterPlayers = [.. AllRosterPlayers.Where(p => p.Id != player.Id)];
            ApplyPositionFilter();
            UpdateSalaryCap();
        }

        [RelayCommand]
        public async Task ClearAllRoster()
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmación",
                "¿Está seguro de que desea quitar a todos los jugadores de la plantilla?",
                "Sí", "No");

            if (confirm)
            {
                AllRosterPlayers = [];
                ApplyPositionFilter();
                UpdateSalaryCap();
            }
        }

        [RelayCommand]
        public async Task SaveRoster()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                TeamDto teamDto = new() { Id = TeamId };

                var rosteredPlayers = AllRosterPlayers.Select(p => new RosteredDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    APY = p.APY,
                    PureAPY = p.Player.PureAPY,
                    FranchiseId = 0
                }).ToList();

                teamDto.Players = rosteredPlayers;
                teamDto.SelectedIds = [.. AllRosterPlayers.Select(p => p.Id)];

                if (await _teamService.UpdateRosterAsync(teamDto))
                    await BaseService.GoToMyTeamsTabAsync();
                else
                    ErrorMessage = "Error al guardar la plantilla";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error en el guardado: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyPositionFilter() => FilteredPlayers = SelectedPositionFilter == "All"
                ? AllRosterPlayers
                : [.. AllRosterPlayers.Where(p => p.Position == SelectedPositionFilter)];

        private void UpdateSalaryCap()
        {
            CurrentSalaryCap = 0;
            foreach (var player in AllRosterPlayers)
                if (decimal.TryParse(player.Player.PureAPY, out decimal salary))
                    CurrentSalaryCap += salary;

            OnPropertyChanged(nameof(AvailableCap));
            OnPropertyChanged(nameof(SalaryCapText));
            OnPropertyChanged(nameof(AvailableCapText));
            OnPropertyChanged(nameof(TotalPlayersCount));
        }
    }
}