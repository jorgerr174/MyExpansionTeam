//using System.Collections.ObjectModel;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using METCore.DTOs.Player;
//using METCore.DTOs.Team;
//using MobileApp.Models.Shared;
//using MobileApp.Services;
//using static METCore.Enums.Types;

//namespace MobileApp.Models.Team
//{
//    public partial class RosterViewModel(TeamService teamService) : TeamBaseViewModel
//    {
//        private readonly TeamService _teamService = teamService;

//        [ObservableProperty] private int teamId;
//        [ObservableProperty] private TeamDto? team = null;
//        [ObservableProperty] private bool hasTeam = false;

//        // Local state management
//        private IList<int> _rosterPlayerIds = [];
//        private IList<int> _protectedPlayerIds = [];
//        private IList<int> _tradedPlayerIds = [];
//        private double _salaryCapLimit = 224000000; // $224M base
//        private int _maxPerFranchise = 4;

//        // Tab Management
//        [ObservableProperty] private string selectedTab = "build";
//        [ObservableProperty] private double tabIndicatorPosition = 0;

//        // Tab Colors
//        [ObservableProperty] private Color buildTabColor = Colors.White;
//        [ObservableProperty] private Color buildTabTextColor = Color.FromArgb("#007bff");
//        [ObservableProperty] private Color reviewTabColor = Color.FromArgb("#f8f9fa");
//        [ObservableProperty] private Color reviewTabTextColor = Color.FromArgb("#6c757d");
//        [ObservableProperty] private Color formationTabColor = Color.FromArgb("#f8f9fa");
//        [ObservableProperty] private Color formationTabTextColor = Color.FromArgb("#6c757d");
//        [ObservableProperty] private Color tradesTabColor = Color.FromArgb("#f8f9fa");
//        [ObservableProperty] private Color tradesTabTextColor = Color.FromArgb("#6c757d");
//        [ObservableProperty] private Color draftTabColor = Color.FromArgb("#f8f9fa");
//        [ObservableProperty] private Color draftTabTextColor = Color.FromArgb("#6c757d");

//        // Tab Visibility
//        [ObservableProperty] private bool isBuildTabSelected = true;
//        [ObservableProperty] private bool isReviewTabSelected = false;
//        [ObservableProperty] private bool isFormationTabSelected = false;
//        [ObservableProperty] private bool isTradesTabSelected = false;
//        [ObservableProperty] private bool isDraftTabSelected = false;

//        // Salary Cap Display
//        [ObservableProperty] private double capProgressWidth = 0;
//        [ObservableProperty] private Color capProgressColor = Color.FromArgb("#007bff");
//        [ObservableProperty] private string currentCapText = "$0M / $224M";
//        [ObservableProperty] private string rosterCountText = "0 players";

//        // Build Tab
//        [ObservableProperty] private FranchiseInfo? selectedFranchise;
//        [ObservableProperty] private bool hasSelectedFranchise = false;
//        [ObservableProperty] private string selectedFranchiseTitle = "";
//        [ObservableProperty] private string selectedPositionFilter = "All";
//        [ObservableProperty] private string selectedReviewPositionFilter = "All";

//        // Save functionality
//        [ObservableProperty] private bool canSaveRoster = true;
//        [ObservableProperty] private Color saveButtonColor = Color.FromArgb("#28a745");
//        [ObservableProperty] private bool hasSaveWarning = false;
//        [ObservableProperty] private string saveWarningText = "";

//        // Collections
//        public ObservableCollection<FranchiseModel> Franchises { get; } = [];
//        public ObservableCollection<PositionGroupModel> PlayersByPosition { get; } = [];
//        public ObservableCollection<PositionGroupModel> RosterByPosition { get; } = [];
//        public ObservableCollection<TradeDto> TradeHistory { get; } = [];
//        public ObservableCollection<DraftSelection> DraftResults { get; } = [];

//        // Filter Lists
//        public ObservableCollection<string> PositionFilters { get; } = ["All", "QB", "RB", "WR", "TE", "OL", "DL", "LB", "DB", "K", "P"];
//        public ObservableCollection<string> ReviewPositionFilters { get; } = ["All", "QB", "RB", "WR", "TE", "OL", "DL", "LB", "DB", "K", "P"];

//        [RelayCommand]
//        public override async Task LoadViewAsync(int teamId)
//        {
//            TeamId = teamId;
//            IsLoading = true;
//            HasLoadError = false;

//            try
//            {
//                // Load team data
//                Team = await _teamService.GetTeamAsync(teamId);

//                if (Team != null)
//                {
//                    HasTeam = true;

//                    // Initialize local state from TeamDto
//                    _rosterPlayerIds = Team.Players?.Select(p => p.Id).ToList() ?? [];
//                    _protectedPlayerIds = Team.RosterSettingsProtectedPlayersIds ?? [];
//                    _tradedPlayerIds = Team.TradedPlayers?.Select(p => p.Id).ToList() ?? [];

//                    // Calculate salary cap limit
//                    var capPercentage = Team.RosterSettingsCap;
//                    _salaryCapLimit = (capPercentage / 100.0) * 224000000; // $224M base
//                    _maxPerFranchise = Team.RosterSettingsMaxPerTeam;

//                    // Load franchises
//                    await LoadFranchises();

//                    // Load trade history and draft results in parallel
//                    var tradesTask = LoadTradeHistory();
//                    var draftTask = LoadDraftResults();
//                    await Task.WhenAll(tradesTask, draftTask);

//                    // Update displays
//                    UpdateSalaryCapDisplay();
//                    await LoadRosterDisplay();

//                    HasLoadError = false;
//                }
//                else
//                {
//                    HasLoadError = true;
//                    LoadErrorMessage = "Team not found";
//                }
//            }
//            catch (Exception ex)
//            {
//                HasLoadError = true;
//                LoadErrorMessage = $"Failed to load roster: {ex.Message}";
//            }
//            finally
//            {
//                IsLoading = false;
//            }
//        }

//        private async Task LoadFranchises()
//        {
//            Franchises.Clear();
//            foreach (FranchiseInfo franchise in FranchiseInfo.GetAllFranchises())
//                Franchises.Add(new(franchise, await GetFranchiseSelectedCount(franchise.Id)));
//        }

//        private async Task<int> GetFranchiseSelectedCount(int franchiseId)
//        {
//            try
//            {
//                var players = await _teamService.GetSelectablePlayersAsync(franchiseId);
//                if (players == null) return 0;

//                return players.Count(p => _rosterPlayerIds.Contains(p.Id));
//            }
//            catch
//            {
//                return 0;
//            }
//        }

//        private async Task LoadTradeHistory()
//        {
//            try
//            {
//                var trades = await _teamService.GetTeamTradesAsync(TeamId);
//                TradeHistory.Clear();
//                if (trades != null)
//                {
//                    foreach (var trade in trades)
//                        TradeHistory.Add(trade);
//                }
//            }
//            catch { }
//        }

//        private async Task LoadDraftResults()
//        {
//            try
//            {
//                if (await _teamService.GetTeamDraftAsync(TeamId) is DraftDto draft && (draft.Selections?.Any() ?? false))
//                    foreach (KeyValuePair<int, int> selection in draft.Selections)
//                        if (draft.Prospects.FirstOrDefault(p => p.Id == selection.Value) is ProspectDto prospect)
//                        {
//                            (int round, int pos) = DraftPicks.GetPickRoundPosFromOverall(selection.Key);
//                            DraftResults.Add(new DraftSelection() { Pick = $"Round: {round}, Pick: {pos}", Player = prospect });
//                        }
//            }
//            catch { }
//        }

//        [RelayCommand]
//        public async Task SelectTab(string tabName)
//        {
//            // Reset all tabs
//            IsBuildTabSelected = false;
//            IsReviewTabSelected = false;
//            IsFormationTabSelected = false;
//            IsTradesTabSelected = false;
//            IsDraftTabSelected = false;

//            // Reset colors
//            BuildTabColor = Color.FromArgb("#f8f9fa");
//            BuildTabTextColor = Color.FromArgb("#6c757d");
//            ReviewTabColor = Color.FromArgb("#f8f9fa");
//            ReviewTabTextColor = Color.FromArgb("#6c757d");
//            FormationTabColor = Color.FromArgb("#f8f9fa");
//            FormationTabTextColor = Color.FromArgb("#6c757d");
//            TradesTabColor = Color.FromArgb("#f8f9fa");
//            TradesTabTextColor = Color.FromArgb("#6c757d");
//            DraftTabColor = Color.FromArgb("#f8f9fa");
//            DraftTabTextColor = Color.FromArgb("#6c757d");

//            // Set selected tab
//            SelectedTab = tabName;

//            switch (tabName)
//            {
//                case "build":
//                    IsBuildTabSelected = true;
//                    BuildTabColor = Colors.White;
//                    BuildTabTextColor = Color.FromArgb("#007bff");
//                    TabIndicatorPosition = 0;
//                    break;
//                case "review":
//                    IsReviewTabSelected = true;
//                    ReviewTabColor = Colors.White;
//                    ReviewTabTextColor = Color.FromArgb("#007bff");
//                    TabIndicatorPosition = 70;
//                    await LoadRosterDisplay();
//                    break;
//                case "formation":
//                    IsFormationTabSelected = true;
//                    FormationTabColor = Colors.White;
//                    FormationTabTextColor = Color.FromArgb("#007bff");
//                    TabIndicatorPosition = 140;
//                    break;
//                case "trades":
//                    IsTradesTabSelected = true;
//                    TradesTabColor = Colors.White;
//                    TradesTabTextColor = Color.FromArgb("#007bff");
//                    TabIndicatorPosition = 210;
//                    break;
//                case "draft":
//                    IsDraftTabSelected = true;
//                    DraftTabColor = Colors.White;
//                    DraftTabTextColor = Color.FromArgb("#007bff");
//                    TabIndicatorPosition = 280;
//                    break;
//            }
//        }

//        [RelayCommand]
//        public async Task SelectFranchise(FranchiseInfo franchise)
//        {
//            if (franchise == null) return;

//            SelectedFranchise = franchise;
//            HasSelectedFranchise = true;
//            SelectedFranchiseTitle = $"🏈 {franchise.Abbreviation} - {franchise.Name}";

//            // Update franchise selection display
//            foreach (var franchiseDisplay in Franchises)
//            {
//                franchiseDisplay.BackgroundColor = franchiseDisplay.FranchiseInfo.Id == franchise.Id
//                    ? Color.FromArgb("#d4edda")
//                    : Color.FromArgb("#f8f9fa");
//            }

//            // Load players for this franchise
//            await LoadFranchisePlayers(franchise.Id);
//        }

//        private async Task LoadFranchisePlayers(int franchiseId)
//        {
//            try
//            {
//                IList<SelectableDto>? dtos = await _teamService.GetSelectablePlayersAsync(franchiseId);
//                if (dtos is null) return;

//                PlayerModel player;
//                IList<PlayerModel>? players = [];
//                foreach (SelectableDto p in dtos)
//                    player = new(p, _tradedPlayerIds.Contains(p.Id), _protectedPlayerIds.Contains(p.Id), _rosterPlayerIds.Contains(p.Id));

//                // Group by position and apply filter
//                IList<IGrouping<string, PlayerModel>> playersByPosition = 
//                    players.GroupBy(p => p.Player.Position).OrderBy(g => g.Key).ToList();

//                if(SelectedPositionFilter != "All")
//                    playersByPosition = [..playersByPosition.Where(pbp => pbp.Key == SelectedPositionFilter)];

//                PlayersByPosition.Clear();
//                foreach (IGrouping<string, PlayerModel> group in playersByPosition)
//                    PlayersByPosition.Add(new([.. group]));
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("Error", $"Failed to load players: {ex.Message}", "OK");
//            }
//        }

//        [RelayCommand]
//        public async Task TogglePlayer(PlayerModel player)
//        {
//            if (player is null || !player.Clickable) return;

//            player.TogglePlayer();
//            UpdateSalaryCapDisplay();
//            await LoadFranchises();
//        }

//        [RelayCommand]
//        public async Task RemovePlayer(SelectableDto player)
//        {
//            if (player == null) return;

//            // Cannot remove protected players
//            if (_protectedPlayerIds.Contains(player.Id))
//            {
//                await Shell.Current.DisplayAlert("Protected Player",
//                    "This player is protected and cannot be removed.", "OK");
//                return;
//            }

//            var confirm = await Shell.Current.DisplayAlert(
//                "Remove Player",
//                $"Remove {player.Name} from roster?",
//                "Yes", "No");

//            if (!confirm) return;

//            // Remove from local roster
//            _rosterPlayerIds.Remove(player.Id);

//            // Update displays
//            UpdateSalaryCapDisplay();
//            await LoadRosterDisplay();
//            await LoadFranchises();
//        }

//        private async Task LoadRosterDisplay()
//        {
//            try
//            {
//                // Get all current roster players from all franchises
//                var allRosterPlayers = new List<SelectableDto>();
//                var franchises = FranchiseInfo.GetAllFranchises();

//                foreach (var franchise in franchises)
//                {
//                    var players = await _teamService.GetSelectablePlayersAsync(franchise.Id);
//                    if (players != null)
//                    {
//                        var rosterPlayers = players.Where(p => _rosterPlayerIds.Contains(p.Id));
//                        allRosterPlayers.AddRange(rosterPlayers);
//                    }
//                }

//                // Group by position and apply filter
//                var filteredPlayers = SelectedReviewPositionFilter == "All"
//                    ? allRosterPlayers
//                    : allRosterPlayers.Where(p => p.Position == SelectedReviewPositionFilter);

//                var playersByPosition = filteredPlayers
//                    .GroupBy(p => p.Position)
//                    .OrderBy(g => g.Key)
//                    .ToList();

//                RosterByPosition.Clear();
//                foreach (var group in playersByPosition)
//                {
//                    PositionGroupModel positionGroup = new(group.Key);

//                    foreach (var player in group.OrderBy(p => p.Name))
//                    {
//                        PlayerModel displayInfo = new(player);
//                        displayInfo.CanRemove = !_protectedPlayerIds.Contains(player.Id);

//                        // Set status text for roster view
//                        if (_protectedPlayerIds.Contains(player.Id))
//                        {
//                            displayInfo.HasStatus = true;
//                            displayInfo.StatusText = "Protected player - cannot be removed";
//                        }

//                        positionGroup.Players.Add(displayInfo);
//                    }

//                    RosterByPosition.Add(positionGroup);
//                }

//                RosterCountText = $"{allRosterPlayers.Count} players";
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("Error", $"Failed to load roster display: {ex.Message}", "OK");
//            }
//        }

//        private void UpdateSalaryCapDisplay()
//        {
//            var currentCapUsed = CalculateCurrentCapUsed();
//            var capPercentage = (currentCapUsed / _salaryCapLimit) * 100;

//            CapProgressWidth = Math.Min(capPercentage * 3, 300); // Max width for progress bar
//            CurrentCapText = $"${currentCapUsed / 1000000:F1}M / ${_salaryCapLimit / 1000000:F0}M";

//            // Update cap progress color based on usage
//            if (capPercentage > 100)
//            {
//                CapProgressColor = Color.FromArgb("#dc3545"); // Red
//                HasSaveWarning = true;
//                SaveWarningText = "⚠️ Salary cap exceeded! Reduce roster cost before saving.";
//                CanSaveRoster = false;
//                SaveButtonColor = Color.FromArgb("#dc3545");
//            }
//            else if (capPercentage > 90)
//            {
//                CapProgressColor = Color.FromArgb("#ffc107"); // Yellow
//                HasSaveWarning = false;
//                CanSaveRoster = true;
//                SaveButtonColor = Color.FromArgb("#28a745");
//            }
//            else
//            {
//                CapProgressColor = Color.FromArgb("#28a745"); // Green
//                HasSaveWarning = false;
//                CanSaveRoster = true;
//                SaveButtonColor = Color.FromArgb("#28a745");
//            }
//        }

//        private double CalculateCurrentCapUsed()
//        {
//            // This would need to calculate based on current roster
//            // For now, return 0 as we'd need to fetch all roster players to calculate
//            return 0; // TODO: Implement proper cap calculation
//        }

//        private double ParseAPY(string apy)
//        {
//            var cleaned = apy.Replace("$", "").Replace("M", "").Replace("K", "").Replace(",", "");
//            if (double.TryParse(cleaned, out double value))
//            {
//                return apy.Contains("M") ? value * 1000000 :
//                       apy.Contains("K") ? value * 1000 : value;
//            }
//            return 0;
//        }

//        [RelayCommand]
//        public async Task SaveRoster()
//        {
//            if (Team == null) return;

//            try
//            {
//                IsLoading = true;

//                // Update TeamDto with current roster selection
//                Team.SelectedIds = _rosterPlayerIds;

//                var success = await _teamService.UpdateRosterAsync(Team);
//                if (success)
//                {
//                    await Shell.Current.DisplayAlert("Success", "Roster saved successfully!", "OK");
//                }
//                else
//                {
//                    await Shell.Current.DisplayAlert("Error", "Failed to save roster.", "OK");
//                }
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("Error", $"Error saving roster: {ex.Message}", "OK");
//            }
//            finally
//            {
//                IsLoading = false;
//            }
//        }

//        [RelayCommand]
//        public async Task ClearRoster()
//        {
//            var confirm = await Shell.Current.DisplayAlert(
//                "Clear Roster",
//                "This will remove all non-protected players from your roster. Protected players will remain. Are you sure?",
//                "Yes", "No");

//            if (!confirm) return;

//            try
//            {
//                // Remove all non-protected players
//                _rosterPlayerIds = _rosterPlayerIds.Where(id => _protectedPlayerIds.Contains(id)).ToList();

//                // Update displays
//                UpdateSalaryCapDisplay();
//                await LoadRosterDisplay();
//                await LoadFranchises();

//                await Shell.Current.DisplayAlert("Success", "Roster cleared successfully! Protected players remain.", "OK");
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("Error", $"Failed to clear roster: {ex.Message}", "OK");
//            }
//        }

//        [RelayCommand]
//        public async Task NewTrade()
//        {
//            if (Team == null) return;

//            await BaseService.GoToAsync(AppRoutes.Trade, new Dictionary<string, object>
//            {
//                ["teamId"] = Team.Id,
//                ["context"] = "roster"
//            });
//        }

//        [RelayCommand]
//        public async Task GoToFormation()
//        {
//            if (Team == null) return;

//            await BaseService.GoToAsync(AppRoutes.Formation, new Dictionary<string, object>
//            {
//                ["TeamId"] = Team.Id
//            });
//        }

//        [RelayCommand]
//        public async Task GoToDraft()
//        {
//            if (Team == null) return;

//            await BaseService.GoToAsync(AppRoutes.Draft, new Dictionary<string, object>
//            {
//                ["TeamId"] = Team.Id
//            });
//        }

//        // Property change handlers for filters
//        partial void OnSelectedPositionFilterChanged(string value)
//        {
//            if (HasSelectedFranchise && SelectedFranchise != null)
//            {
//                _ = Task.Run(async () => await LoadFranchisePlayers(SelectedFranchise.Id));
//            }
//        }

//        partial void OnSelectedReviewPositionFilterChanged(string value)
//        {
//            _ = Task.Run(async () => await LoadRosterDisplay());
//        }
//    }
//}