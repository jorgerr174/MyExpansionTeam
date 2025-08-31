using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Team
{
    public partial class RosterViewModel(TeamService teamService) : TeamBaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private TeamDto? team = null;
        [ObservableProperty] private bool hasTeam = false;

        // Local state management
        private IList<int> _rosterPlayerIds = [];
        private IList<int> _protectedPlayerIds = [];
        private IList<int> _tradedPlayerIds = [];
        private double _salaryCapLimit = 224000000; // $224M base
        private int _maxPerFranchise = 3;

        // Caching
        private Dictionary<int, IList<SelectableDto>?> _franchisePlayersCache = [];

        // Tab Management
        [ObservableProperty] private string selectedTab = "build";
        [ObservableProperty] private double tabIndicatorPosition = 0;

        // Tab Colors
        [ObservableProperty] private Color buildTabColor = Colors.White;
        [ObservableProperty] private Color buildTabTextColor = Color.FromArgb("#007bff");
        [ObservableProperty] private Color reviewTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color reviewTabTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color formationTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color formationTabTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color tradesTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color tradesTabTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color draftTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color draftTabTextColor = Color.FromArgb("#6c757d");

        // Tab Visibility
        [ObservableProperty] private bool isBuildTabSelected = true;
        [ObservableProperty] private bool isReviewTabSelected = false;
        [ObservableProperty] private bool isFormationTabSelected = false;
        [ObservableProperty] private bool isTradesTabSelected = false;
        [ObservableProperty] private bool isDraftTabSelected = false;

        // Salary Cap Display
        [ObservableProperty] private double capProgressWidth = 0;
        [ObservableProperty] private Color capProgressColor = Color.FromArgb("#007bff");
        [ObservableProperty] private string currentCapText = "$0M / $224M";
        [ObservableProperty] private string rosterCountText = "0 jugadores";

        // Build Tab
        [ObservableProperty] private FranchiseModel? selectedFranchise;
        [ObservableProperty] private bool hasSelectedFranchise = false;
        [ObservableProperty] private string selectedFranchiseTitle = "";
        [ObservableProperty] private string selectedPositionFilter = "All";
        [ObservableProperty] private string selectedReviewPositionFilter = "All";

        // Save functionality
        [ObservableProperty] private bool canSaveRoster = true;
        [ObservableProperty] private Color saveButtonColor = Color.FromArgb("#28a745");
        [ObservableProperty] private bool hasSaveWarning = false;
        [ObservableProperty] private string saveWarningText = "";

        // Formation Management Properties
        [ObservableProperty] private string viewedFormationType = "offense";
        [ObservableProperty] private Color offenseViewColor = Color.FromArgb("#007bff");
        [ObservableProperty] private Color offenseViewTextColor = Colors.White;
        [ObservableProperty] private Color defenseViewColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color defenseViewTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color specialViewColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color specialViewTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private string currentFormationDisplayName = "";
        [ObservableProperty] private bool isOffenseViewSelected = true;
        [ObservableProperty] private bool isDefenseViewSelected = false;
        [ObservableProperty] private bool isSpecialViewSelected = false;

        // Collections
        public ObservableCollection<FranchiseModel> Franchises { get; } = [];
        public ObservableCollection<PositionGroupModel> PlayersByPosition { get; } = [];
        public ObservableCollection<PositionGroupModel> RosterByPosition { get; } = [];

        // Formation Collections
        public ObservableCollection<string> AvailableFormations { get; } = [];
        public ObservableCollection<FormationPosition> FormationPositions { get; } = [];
        public ObservableCollection<DraggablePlayer> AvailablePlayersForFormation { get; } = [];
        // Formation Data Storage
        public Thickness FDylMargin => new(10, IsDefenseViewSelected ? 90 : 10);
        public LayoutOptions SylVo => new(IsDefenseViewSelected ? LayoutAlignment.End : LayoutAlignment.Start, true);
        public Thickness SylMargin => new(10, IsDefenseViewSelected ? 60 : 95);

        private LineupDto _offenseLineup = new();
        private LineupDto _defenseLineup = new();
        private SPLineupDto _specialLineup = new();
        private Dictionary<string, Dictionary<string, FormationInfo>> _formationData = [];
        private DraggablePlayer? _draggedPlayer;


        [ObservableProperty] private IList<TradeDto> tradeHistory = [];
        [ObservableProperty] private IList<DraftSelection> draftResults = [];
        [ObservableProperty] private DraftDto draft;
        [ObservableProperty] private bool notHasDraft = true;

        // Filter Lists
        public ObservableCollection<string> PositionFilters { get; } = ["All", "QB", "RB", "WR", "TE", "OL", "DL", "LB", "DB", "K", "P"];
        public ObservableCollection<string> ReviewPositionFilters { get; } = ["All", "QB", "RB", "WR", "TE", "OL", "DL", "LB", "DB", "K", "P"];

        private bool AreFormationsLoaded => _formationData.Any() && AvailableFormations.Any();

        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            IsLoading = true;
            HasLoadError = false;

            try
            {
                // Load team data
                Team = await _teamService.GetTeamAsync(teamId);

                if (Team != null)
                {
                    HasTeam = true;

                    _rosterPlayerIds = Team.Players?.Select(p => p.Id).ToList() ?? [];
                    _protectedPlayerIds = Team.RosterSettingsProtectedPlayersIds ?? [];
                    _tradedPlayerIds = Team.TradedPlayers?.Select(p => p.Id).ToList() ?? [];

                    var capPercentage = Team.RosterSettingsCap;
                    _salaryCapLimit = (capPercentage / 100.0) * 224000000; // $224M base
                    _maxPerFranchise = Team.RosterSettingsMaxPerTeam;

                    LoadFranchises();

                    UpdateSalaryCapDisplay();
                    LoadRosterDisplay();

                    HasLoadError = false;
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
                LoadErrorMessage = $"Failed to load roster: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private int GetFranchiseSelectedCount(int franchiseId) => _franchisePlayersCache[franchiseId]?.Count(p => _rosterPlayerIds.Contains(p.Id)) ?? 0;

        private void LoadFranchises()
        {
            Franchises.Clear();
            foreach (FranchiseInfo franchise in FranchiseInfo.GetAllFranchises())
                Franchises.Add(new(franchise, 0, _maxPerFranchise));
        }

        private void UpdateFranchiseCount(int franchiseId)
        {
            if (Franchises.Where(f => f.FranchiseInfo.Id == franchiseId) is FranchiseModel franchise)
                franchise.SelectedCount = GetFranchiseSelectedCount(franchise.FranchiseInfo.Id);
        }

        private void UpdateFranchisesCounts()
        {
            foreach (FranchiseModel franchise in Franchises)
                franchise.SelectedCount = GetFranchiseSelectedCount(franchise.FranchiseInfo.Id);
        }

        private async Task LoadDraftResults()
        {
            try
            {
                Draft = await _teamService.GetTeamDraftAsync(Team.Id);
                if (Draft is DraftDto && (Draft.Selections?.Any() ?? false))
                {
                    NotHasDraft = false;
                    if (Draft.Selections?.Any() ?? false)
                        foreach (KeyValuePair<int, int> selection in Draft.Selections)
                            if (Draft.Prospects.FirstOrDefault(p => p.Id == selection.Value) is ProspectDto prospect)
                            {
                                (int round, int pos) = DraftPicks.GetPickRoundPosFromOverall(selection.Key);
                                DraftResults.Add(new DraftSelection() { Pick = $"Ronda: {round}, Pick: {pos}", Player = prospect });
                            }
                }
            }
            catch { }
        }

        [RelayCommand]
        public async Task SelectTab(string tabName)
        {
            // Reset all tabs
            IsBuildTabSelected = false;
            IsReviewTabSelected = false;
            IsFormationTabSelected = false;
            IsTradesTabSelected = false;
            IsDraftTabSelected = false;

            // Reset colors
            BuildTabColor = Color.FromArgb("#f8f9fa");
            BuildTabTextColor = Color.FromArgb("#6c757d");
            ReviewTabColor = Color.FromArgb("#f8f9fa");
            ReviewTabTextColor = Color.FromArgb("#6c757d");
            FormationTabColor = Color.FromArgb("#f8f9fa");
            FormationTabTextColor = Color.FromArgb("#6c757d");
            TradesTabColor = Color.FromArgb("#f8f9fa");
            TradesTabTextColor = Color.FromArgb("#6c757d");
            DraftTabColor = Color.FromArgb("#f8f9fa");
            DraftTabTextColor = Color.FromArgb("#6c757d");

            // Set selected tab
            SelectedTab = tabName;

            switch (tabName)
            {
                case "build":
                    IsBuildTabSelected = true;
                    BuildTabColor = Colors.White;
                    BuildTabTextColor = Color.FromArgb("#007bff");
                    TabIndicatorPosition = 0;
                    break;
                case "review":
                    IsReviewTabSelected = true;
                    ReviewTabColor = Colors.White;
                    ReviewTabTextColor = Color.FromArgb("#007bff");
                    TabIndicatorPosition = 70;
                    LoadRosterDisplay();
                    OnPropertyChanged(nameof(RosterByPosition));
                    break;
                case "formation":
                    IsFormationTabSelected = true;
                    FormationTabColor = Colors.White;
                    FormationTabTextColor = Color.FromArgb("#007bff");
                    TabIndicatorPosition = 140;
                    IsOffenseViewSelected = false;
                    IsDefenseViewSelected = false;
                    IsSpecialViewSelected = false;
                    switch (ViewedFormationType)
                    {
                        case "offense":
                            OffenseViewColor = Color.FromArgb("#007bff");
                            OffenseViewTextColor = Colors.White;
                            IsOffenseViewSelected = true;
                            break;
                        case "defense":
                            DefenseViewColor = Color.FromArgb("#007bff");
                            DefenseViewTextColor = Colors.White;
                            IsDefenseViewSelected = true;
                            break;
                        case "special":
                            SpecialViewColor = Color.FromArgb("#007bff");
                            SpecialViewTextColor = Colors.White;
                            IsSpecialViewSelected = true;
                            break;
                    }
                    if (!_formationData.Any())
                    {
                        LoadFormationData();
                        LoadCurrentLineups();
                        UpdateFormationTypeButtons();
                        LoadAvailableFormations();
                    }
                    else
                        LoadFormationPositions(); // Just refresh positions if already loaded
                    OnPropertyChanged(nameof(FDylMargin));
                    OnPropertyChanged(nameof(SylVo));
                    OnPropertyChanged(nameof(SylMargin));
                    break;
                case "trades":
                    TradeHistory = await _teamService.GetTeamTradesAsync(Team?.Id ?? 0) ?? [];
                    IsTradesTabSelected = true;
                    TradesTabColor = Colors.White;
                    TradesTabTextColor = Color.FromArgb("#007bff");
                    TabIndicatorPosition = 210;
                    break;
                case "draft":
                    await LoadDraftResults();
                    IsDraftTabSelected = true;
                    DraftTabColor = Colors.White;
                    DraftTabTextColor = Color.FromArgb("#007bff");
                    TabIndicatorPosition = 280;
                    break;
            }
        }

        [RelayCommand]
        public async Task SelectFranchise(FranchiseInfo franchiseInfo)
        {
            if (franchiseInfo is FranchiseInfo && Franchises.FirstOrDefault(f => f.FranchiseInfo.Id == franchiseInfo.Id) is FranchiseModel franchise)
            {
                SelectedFranchise = franchise;
                HasSelectedFranchise = true;
                SelectedFranchiseTitle = $"🏈 {franchiseInfo.Abbreviation} - {franchiseInfo.Name}";

                foreach (FranchiseModel franchiseDisplay in Franchises)
                    franchiseDisplay.BackgroundColor = franchiseDisplay.FranchiseInfo.Id == franchiseInfo.Id ? Color.FromArgb("#d4edda") : Color.FromArgb("#f8f9fa");

                await LoadFranchisePlayers(franchiseInfo.Id);
            }
        }

        private async Task LoadFranchisePlayers(int franchiseId)
        {
            try
            {
                // Use cached data or load from API
                IList<SelectableDto> dtos;
                if (_franchisePlayersCache.ContainsKey(franchiseId))
                    dtos = _franchisePlayersCache[franchiseId];
                else
                {
                    dtos = await _teamService.GetSelectablePlayersAsync(franchiseId);
                    if (dtos is null) return;
                    _franchisePlayersCache.Add(franchiseId, dtos);
                }

                IList<PlayerModel> players = [];
                foreach (SelectableDto p in dtos ?? [])
                {
                    players.Add(new(p, _tradedPlayerIds.Contains(p.Id), _protectedPlayerIds.Contains(p.Id), _rosterPlayerIds.Contains(p.Id)));
                }

                var playersByPosition = players.GroupBy(p => p.Player.Position).ToList();
                if (SelectedPositionFilter != "All")
                    playersByPosition = [.. playersByPosition.Where(pbp => pbp.Key == SelectedPositionFilter)];

                PlayersByPosition.Clear();
                foreach (var group in playersByPosition)
                    PlayersByPosition.Add(new([.. group.OrderBy(p => p.Player.Name)]));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar jugadores: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task TogglePlayer(PlayerModel player)
        {
            if (player is PlayerModel && player.Clickable && SelectedFranchise is FranchiseModel && SelectedFranchise.FranchiseInfo is FranchiseInfo)
            {
                if (player.StatusAvailable && GetFranchiseSelectedCount(SelectedFranchise.FranchiseInfo.Id) >= _maxPerFranchise)
                {
                    await Shell.Current.DisplayAlert("Límite por Franquicia",
                        $"Solo puede seleccionar {_maxPerFranchise} jugadores por franquicia.", "OK");
                    return;
                }
                else
                {
                    player.TogglePlayer();
                    if (player.IsSelected)
                        _rosterPlayerIds.Add(player.Id);
                    else if (player.StatusAvailable)
                        _rosterPlayerIds.Remove(player.Id);

                    UpdateSalaryCapDisplay();
                    UpdateFranchiseCount(SelectedFranchise.FranchiseInfo.Id);
                }
            }

        }

        [RelayCommand]
        public async Task RemovePlayer(PlayerModel player)
        {
            if (player == null) return;

            // Cannot remove protected players
            if (_protectedPlayerIds.Contains(player.Id))
            {
                await Shell.Current.DisplayAlert("Jugador Protegido",
                    "Este jugador fue obtenido via Trueque y no puede ser deseleccionado.", "OK");
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Quitar Jugador",
                $"Quitar {player.Player.Name} de la plantilla?",
                "Sí", "No");

            if (!confirm) return;

            // Remove from local roster
            _rosterPlayerIds.Remove(player.Id);

            // Update displays
            UpdateSalaryCapDisplay();
            LoadRosterDisplay();
            UpdateFranchisesCounts();
        }

        public void LoadRosterDisplay()
        {
            try
            {
                // Build roster display from cached data
                IList<SelectableDto> rosterPlayers = [];
                for (int i = 1; i < 33; i++)
                {
                    rosterPlayers =
                        rosterPlayers.Concat(
                        _franchisePlayersCache.TryGetValue(i, out IList<SelectableDto> list)
                        ? list.Where(p => _rosterPlayerIds.Contains(p.Id)) : Team.Players.Where(p => p.FranchiseId == i).ToList())
                        .ToList();
                }
                rosterPlayers = rosterPlayers.Concat(Team.Players.Where(p => p.FranchiseId == 0)).ToList();

                IList<PlayerModel> allRosterPlayers = [];
                foreach (SelectableDto player in rosterPlayers)
                    allRosterPlayers.Add(new PlayerModel(player, false, _protectedPlayerIds.Contains(player.Id), true));

                var filteredPlayers = SelectedReviewPositionFilter == "All"
                    ? allRosterPlayers
                    : allRosterPlayers.Where(p => p.Player.Position == SelectedReviewPositionFilter);

                var playersByPosition = filteredPlayers
                    .GroupBy(p => p.Player.Position)
                    .ToList();

                RosterByPosition.Clear();
                foreach (var group in playersByPosition)
                    RosterByPosition.Add(new([.. group.OrderBy(p => p.Player.Name)]));

                RosterCountText = $"{allRosterPlayers.Count} jugadores";
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", $"Error al cargar la plantilla: {ex.Message}", "OK");
            }
        }

        private void UpdateSalaryCapDisplay()
        {
            var currentCapUsed = CalculateCurrentCapUsed();
            var capPercentage = currentCapUsed / (_salaryCapLimit / 1000000);

            CapProgressWidth = Math.Min(capPercentage * 3, 300); // Max width for progress bar
            CurrentCapText = $"${currentCapUsed:F2}M / ${_salaryCapLimit / 1000000:F0}M";

            if (capPercentage > 100)
            {
                CapProgressColor = Color.FromArgb("#dc3545"); // Red
                HasSaveWarning = true;
                SaveWarningText = "⚠️ Salary cap exceeded! Reduce roster cost before saving.";
                CanSaveRoster = false;
                SaveButtonColor = Color.FromArgb("#dc3545");
            }
            else if (capPercentage > 90)
            {
                CapProgressColor = Color.FromArgb("#ffc107");
                HasSaveWarning = false;
                CanSaveRoster = true;
                SaveButtonColor = Color.FromArgb("#28a745");
            }
            else
            {
                CapProgressColor = Color.FromArgb("#28a745"); // Green
                HasSaveWarning = false;
                CanSaveRoster = true;
                SaveButtonColor = Color.FromArgb("#28a745");
            }
        }

        private double CalculateCurrentCapUsed()
        {
            double totalCap = 0;
            for (int i = 1; i < 33; i++)
            {
                if (_franchisePlayersCache.TryGetValue(i, out IList<SelectableDto> list))
                    foreach (SelectableDto player in list.Where(p => _rosterPlayerIds.Contains(p.Id)))
                        totalCap += ParseAPY(player.PureAPY);
                else if (Team is TeamDto)
                    foreach (RosteredDto player in Team.Players.Where(p => p.FranchiseId == i))
                        totalCap += ParseAPY(player.PureAPY);
            }

            foreach (RosteredDto player in Team.Players.Where(p => p.FranchiseId == 0))
                totalCap += ParseAPY(player.PureAPY);

            return totalCap;
        }
        private double ParseAPY(string pureAPY) => double.TryParse(pureAPY.Replace(".", ","), out double value) ? value / 100 : 0;

        [RelayCommand]
        public async Task SaveRoster()
        {
            if (Team == null) return;

            try
            {
                IsLoading = true;

                Team.SelectedIds = _rosterPlayerIds;
                Team.OffLineup = _offenseLineup;
                Team.DefLineup = _defenseLineup;
                Team.SPLineup = _specialLineup;

                Team.SelectedIds = _rosterPlayerIds;

                var success = await _teamService.UpdateRosterAsync(Team);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Éxito", "!Plantilla guardada con éxito!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error al guardar la plantilla.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al guardar la plantilla: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ClearRoster()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Vaciar Plantilla",
                "Esto quitará a todos los jugadores de la plantilla. ¿Estás seguro?",
                "Sí", "No");

            if (!confirm) return;

            try
            {
                // Remove all non-protected players
                _rosterPlayerIds = _rosterPlayerIds.Where(id => _protectedPlayerIds.Contains(id)).ToList();

                // Update displays
                UpdateSalaryCapDisplay();
                LoadRosterDisplay();
                UpdateFranchisesCounts();

                await Shell.Current.DisplayAlert("Éxito", "Plantilla vaciada con éxito!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error vaciando la plantilla: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task NewTrade()
            => await BaseService.GoToAsync(AppRoutes.Trade, new() { ["TeamId"] = Team.Id, ["CurrentPick"] = -1 });

        [RelayCommand]
        public async Task GoToDraft()
        {
            if (Team == null) return;

            await BaseService.GoToAsync(AppRoutes.Draft, new Dictionary<string, object> { ["TeamId"] = Team.Id });
        }

        partial void OnSelectedPositionFilterChanged(string value)
        {
            if (HasSelectedFranchise && SelectedFranchise is FranchiseModel && SelectedFranchise.FranchiseInfo is FranchiseInfo franchiseInfo)
            {
                _ = Task.Run(async () => await LoadFranchisePlayers(franchiseInfo.Id));
            }
        }

        partial void OnSelectedReviewPositionFilterChanged(string value)
        {
            LoadRosterDisplay();
        }

        [RelayCommand]
        public void SelectFormationType(string formationType)
        {
            ViewedFormationType = formationType;
            UpdateFormationTypeButtons();
            LoadAvailableFormations();
        }

        [RelayCommand]
        public async Task SelectPositionPlayer(FormationPosition position)
        {
            var eligiblePlayers = GetEligiblePlayersForPosition(position.RequiredPosition);

            if (!eligiblePlayers.Any())
            {
                await Shell.Current.DisplayAlert("Sin Jugadores",
                    $"Sin {position.RequiredPosition} jugadores disponibles en la plantilla.", "OK");
                return;
            }

            // Add "Remove Player" option if position is occupied
            var options = eligiblePlayers.Select(p => p.Player.Player.Name).ToList();
            if (position.AssignedPlayer != null)
                options.Insert(0, "🗑️ Remove Player");

            var selectedOption = await Shell.Current.DisplayActionSheet(
                $"Select {position.PositionName} ({position.RequiredPosition})",
                "Cancel", null, options.ToArray());

            if (selectedOption == "Cancel" || string.IsNullOrEmpty(selectedOption)) return;

            if (selectedOption == "🗑️ Remove Player")
            {
                RemovePlayerFromPosition(position);
            }
            else
            {
                var selectedPlayer = eligiblePlayers.FirstOrDefault(p => p.Player.Player.Name == selectedOption);
                if (selectedPlayer != null)
                    AssignPlayerToPosition(selectedPlayer, position);
            }
        }

        public void DropPlayer(FormationPosition position, DraggablePlayer player)
        {
            // Check if player is eligible for this position
            if (!IsPlayerEligibleForPosition(player.Player, position.RequiredPosition))
            {
                Shell.Current.DisplayAlert("Position Inválida",
                    $"{player.Player.Player.Name} no puede jugar de {position.RequiredPosition}", "OK");
                return;
            }

            AssignPlayerToPosition(player, position);
        }

        [RelayCommand]
        public void StartDrag(DraggablePlayer player)
        {
            _draggedPlayer = player;
        }

        // Formation Management Methods
        private void LoadFormationData()
        {
            _formationData = new Dictionary<string, Dictionary<string, FormationInfo>>
            {
                ["offense"] = FormationData.GetOffenseFormations().ToDictionary(f => f.Key, f => f),
                ["defense"] = FormationData.GetDefenseFormations().ToDictionary(f => f.Key, f => f),
                ["special"] = FormationData.GetSpecialTeamsFormations().ToDictionary(f => f.Key, f => f)
            };
        }

        private void LoadCurrentLineups()
        {
            if (Team is not TeamDto) return;

            _offenseLineup =
                Team.OffLineup is LineupDto olineup && !String.IsNullOrWhiteSpace(olineup.Formation)
                ? olineup : new() { Formation = "ZeroOne" };

            _defenseLineup =
                Team.DefLineup is LineupDto dlineup && !String.IsNullOrWhiteSpace(dlineup.Formation)
                ? dlineup : new() { Formation = "Bear" };

            _specialLineup =
                Team.SPLineup is SPLineupDto splineup && !String.IsNullOrWhiteSpace(splineup.Formation)
                ? splineup : new() { Formation = "SpecialTeams" };
        }

        private void UpdateFormationTypeButtons()
        {
            // Reset all colors
            OffenseViewColor = Color.FromArgb("#f8f9fa");
            OffenseViewTextColor = Color.FromArgb("#6c757d");
            DefenseViewColor = Color.FromArgb("#f8f9fa");
            DefenseViewTextColor = Color.FromArgb("#6c757d");
            SpecialViewColor = Color.FromArgb("#f8f9fa");
            SpecialViewTextColor = Color.FromArgb("#6c757d");

            // Set active button
            switch (ViewedFormationType)
            {
                case "offense":
                    OffenseViewColor = Color.FromArgb("#007bff");
                    OffenseViewTextColor = Colors.White;
                    break;
                case "defense":
                    DefenseViewColor = Color.FromArgb("#007bff");
                    DefenseViewTextColor = Colors.White;
                    break;
                case "special":
                    SpecialViewColor = Color.FromArgb("#007bff");
                    SpecialViewTextColor = Colors.White;
                    break;
            }
        }

        private void LoadAvailableFormations()
        {
            AvailableFormations.Clear();

            if (_formationData.ContainsKey(ViewedFormationType))
            {
                foreach (var formation in _formationData[ViewedFormationType].Values)
                {
                    AvailableFormations.Add(formation.Name);
                }

                // Set default formation
                if (AvailableFormations.Any())
                {
                    var currentFormation = GetCurrentFormationName();
                    CurrentFormationDisplayName = AvailableFormations.Contains(currentFormation)
                        ? currentFormation
                        : AvailableFormations.First();
                }
            }

            LoadFormationPositions();
        }

        private string GetCurrentFormationName()
        {
            return ViewedFormationType switch
            {
                "offense" => _offenseLineup.Formation ?? "Eleven",
                "defense" => _defenseLineup.Formation ?? "FourThree",
                "special" => _specialLineup.Formation ?? "SpecialTeams",
                _ => ""
            };
        }

        private void LoadFormationPositions()
        {
            FormationPositions.Clear();
            SPLineupDto lineup = null;
            IList<FormationInfo> formations = [];
            switch (ViewedFormationType)
            {
                case "offense":
                    lineup = _offenseLineup;
                    formations = FormationData.GetOffenseFormations();
                    break;
                case "defense":
                    lineup = _defenseLineup;
                    formations = FormationData.GetDefenseFormations();
                    break;
                case "special":
                    lineup = _specialLineup;
                    formations = FormationData.GetSpecialTeamsFormations();
                    break;
                default: break;
            }

            if (lineup is SPLineupDto && String.IsNullOrWhiteSpace(lineup.Formation) && formations.Any()) return;
            FormationInfo formation = formations.FirstOrDefault(f => f.Key == lineup.Formation);
            if (formation is not FormationInfo || !formation.Positions.Any()) return;

            for (int i = 0; i < formation.Positions.Count; i++)
            {
                var pos = formation.Positions[i];
                FormationPosition formationPos = new(pos.Id, pos.Name, pos.Position, pos.X, pos.Y, i + 1);

                var currentPlayerId = GetLineupPlayerByIndex(lineup, i + 1);
                if (currentPlayerId > 0)
                {
                    var player = FindRosterPlayerById(currentPlayerId);
                    formationPos.AssignPlayer(player);
                }

                FormationPositions.Add(formationPos);
            }

            LoadAvailablePlayersForFormation();
        }

        private static int GetLineupPlayerByIndex(SPLineupDto splineup, int index)
        {
            return splineup is LineupDto lineup
                ? index switch
                {
                    1 => lineup.Player1,
                    2 => lineup.Player2,
                    3 => lineup.Player3,
                    4 => lineup.Player4,
                    5 => lineup.Player5,
                    6 => lineup.Player6,
                    7 => lineup.Player7,
                    8 => lineup.Player8,
                    9 => lineup.Player9,
                    10 => lineup.Player10,
                    11 => lineup.Player11,
                    _ => 0
                }
            : index switch
            {
                1 => splineup.Player1,
                2 => splineup.Player2,
                3 => splineup.Player3,
                4 => splineup.Player4,
                5 => splineup.Player5,
                _ => 0
            };
        }

        private PlayerModel? FindRosterPlayerById(int playerId)
        {
            PlayerModel model = null;

            if (Team is null) return null;
            if (Team.Players.FirstOrDefault(player => player.Id == playerId) is SelectableDto p)
                model = new(p, false, false, true);
            if (_franchisePlayersCache.SelectMany(f => f.Value).ToList().FirstOrDefault(player => player.Id == playerId) is SelectableDto p2)
                model = new(p2, false, false, true);

            return model;
        }

        private void LoadAvailablePlayersForFormation()
        {
            AvailablePlayersForFormation.Clear();

            SPLineupDto lineup = ViewedFormationType switch
            {
                "offense" => _offenseLineup,
                "defense" => _defenseLineup,
                "special" => _specialLineup,
                _ => null,
            };


            if (Team is null) return;

            IList<SelectableDto> rosterPlayers = [];
            for (int i = 1; i < 33; i++)
            {
                rosterPlayers =
                    [
                        .. rosterPlayers,
                        .. (_franchisePlayersCache.TryGetValue(i, out IList<SelectableDto> list)
                        ? list.Where(p => _rosterPlayerIds.Contains(p.Id)) : Team.Players.Where(p => p.FranchiseId == i)).ToList(),
                    ];
            }
            rosterPlayers = rosterPlayers.Concat(Team.Players.Where(p => p.FranchiseId == 0)).ToList();
            IList<int> formationIds = GetAllIdsInLineup(lineup);

            foreach (var player in rosterPlayers.Where(p => !formationIds.Contains(p.Id)).ToList())
                AvailablePlayersForFormation.Add(new DraggablePlayer(new PlayerModel(player, false, false, true)));
        }

        private List<DraggablePlayer> GetEligiblePlayersForPosition(string requiredPosition)
        {
            return AvailablePlayersForFormation
                .Where(p => IsPlayerEligibleForPosition(p.Player, requiredPosition))
                .ToList();
        }

        private static bool IsPlayerEligibleForPosition(PlayerModel player, string requiredPosition)
        {
            var playerPos = player.Player.Position;

            // Position compatibility rules
            return requiredPosition switch
            {
                "QB" => playerPos == "QB",
                "RB" => playerPos == "RB",
                "WR" => playerPos == "WR",
                "TE" => playerPos == "TE",
                "OL" => playerPos == "OL",
                "DL" => playerPos == "DL",
                "LB" => playerPos == "LB",
                "DB" => playerPos == "DB",
                "K" => playerPos == "K",
                "P" => playerPos == "P",
                "LS" => playerPos == "OL", // Long Snapper can be OL
                "ATH" => playerPos is "WR" or "RB" or "DB", // Athletes for returns
                _ => false
            };
        }

        private void AssignPlayerToPosition(DraggablePlayer player, FormationPosition position)
        {
            // Remove player from any other position first
            RemovePlayerFromAllPositions(player.Player);

            // Assign to new position
            position.AssignPlayer(player.Player);

            // Update lineup data
            SetCurrentPlayerForPosition(position.PlayerIndex, player.Player.Id);
        }

        private void RemovePlayerFromPosition(FormationPosition position)
        {
            position.AssignPlayer(null);
            SetCurrentPlayerForPosition(position.PlayerIndex, 0);
        }

        private void RemovePlayerFromAllPositions(PlayerModel player)
        {
            foreach (var pos in FormationPositions)
            {
                if (pos.AssignedPlayer?.Id == player.Id)
                {
                    pos.AssignPlayer(null);
                    SetCurrentPlayerForPosition(pos.PlayerIndex, 0);
                }
            }
        }

        private void SetCurrentPlayerForPosition(int position, int playerId)
        {
            switch (ViewedFormationType)
            {
                case "offense":
                    SetLineupPlayerByIndex(_offenseLineup, position, playerId);
                    break;
                case "defense":
                    SetLineupPlayerByIndex(_defenseLineup, position, playerId);
                    break;
                case "special":
                    SetSPLineupPlayerByIndex(_specialLineup, position, playerId);
                    break;
            }
        }

        private static IList<int> GetAllIdsInLineup(SPLineupDto splineup)
        {
            IList<int> list = [splineup.Player1, splineup.Player2, splineup.Player3, splineup.Player4, splineup.Player5];
            if (splineup is LineupDto lineup)
                list = [.. list, lineup.Player6, lineup.Player7, lineup.Player8, lineup.Player9, lineup.Player10, lineup.Player11];

            return list;
        }

        private static void SetLineupPlayerByIndex(LineupDto lineup, int index, int playerId)
        {
            switch (index)
            {
                case 1: lineup.Player1 = playerId; break;
                case 2: lineup.Player2 = playerId; break;
                case 3: lineup.Player3 = playerId; break;
                case 4: lineup.Player4 = playerId; break;
                case 5: lineup.Player5 = playerId; break;
                case 6: lineup.Player6 = playerId; break;
                case 7: lineup.Player7 = playerId; break;
                case 8: lineup.Player8 = playerId; break;
                case 9: lineup.Player9 = playerId; break;
                case 10: lineup.Player10 = playerId; break;
                case 11: lineup.Player11 = playerId; break;
            }
        }

        private static void SetSPLineupPlayerByIndex(SPLineupDto lineup, int index, int playerId)
        {
            switch (index)
            {
                case 1: lineup.Player1 = playerId; break;
                case 2: lineup.Player2 = playerId; break;
                case 3: lineup.Player3 = playerId; break;
                case 4: lineup.Player4 = playerId; break;
                case 5: lineup.Player5 = playerId; break;
            }
        }


        [RelayCommand]
        public void ViewFormation(string formationType)
        {
            ViewedFormationType = formationType;
            LoadFormationPositions();
            UpdateFormationViewColors();
            UpdateCurrentFormationName();
        }

        private void UpdateFormationViewColors()
        {
            // Reset all to inactive
            OffenseViewColor = Color.FromArgb("#f8f9fa");
            OffenseViewTextColor = Color.FromArgb("#6c757d");
            DefenseViewColor = Color.FromArgb("#f8f9fa");
            DefenseViewTextColor = Color.FromArgb("#6c757d");
            SpecialViewColor = Color.FromArgb("#f8f9fa");
            SpecialViewTextColor = Color.FromArgb("#6c757d");

            IsOffenseViewSelected = false;
            IsDefenseViewSelected = false;
            IsSpecialViewSelected = false;

            // Set active
            switch (ViewedFormationType)
            {
                case "offense":
                    OffenseViewColor = Color.FromArgb("#007bff");
                    OffenseViewTextColor = Colors.White;
                    IsOffenseViewSelected = true;
                    break;
                case "defense":
                    DefenseViewColor = Color.FromArgb("#007bff");
                    DefenseViewTextColor = Colors.White;
                    IsDefenseViewSelected = true;
                    break;
                case "special":
                    SpecialViewColor = Color.FromArgb("#007bff");
                    SpecialViewTextColor = Colors.White;
                    IsSpecialViewSelected = true;
                    break;
            }
        }

        private void UpdateCurrentFormationName()
        {
            string formationKey = string.Empty;
            IList<FormationInfo> formations = [];

            switch (ViewedFormationType)
            {
                case "offense":
                    formationKey = currentFormationDisplayName;
                    formations = FormationData.GetOffenseFormations();
                    break;
                case "defense":
                    formationKey = CurrentFormationDisplayName;
                    formations = FormationData.GetDefenseFormations();
                    break;
                case "special":
                    formationKey = CurrentFormationDisplayName;
                    formations = FormationData.GetSpecialTeamsFormations();
                    break;
            }

            CurrentFormationDisplayName = formations.FirstOrDefault(f => f.Key == formationKey)?.Name ?? "Unknown Formation";
        }
    }
}