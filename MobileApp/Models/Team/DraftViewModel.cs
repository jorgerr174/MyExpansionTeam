using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using METCore.Models.Teams;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Team
{
    public partial class DraftViewModel : TeamBaseViewModel
    {
        private readonly TeamService _teamService;

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = "";
        [ObservableProperty] private string loadingMessage = "Loading...";

        // Draft Setup
        [ObservableProperty] private bool showDraftSettings = true;
        [ObservableProperty] private bool showDraftInterface = false;
        [ObservableProperty] private bool isManualDraft = true;
        [ObservableProperty] private bool isAutoDraft = false;
        [ObservableProperty] private bool isMultipleDraft = false;

        // Draft State
        [ObservableProperty] private int currentPickIndex = 0;
        [ObservableProperty] private string currentPickText = "";
        [ObservableProperty] private bool isPaused = false;
        [ObservableProperty] private bool isSimulating = false;
        [ObservableProperty] private string pauseResumeText = "Pause";
        [ObservableProperty] private bool showPauseButton = false;
        [ObservableProperty] private bool canTrade = false;
        [ObservableProperty] private bool canSaveDraft = false;

        // Current Pick Info
        [ObservableProperty] private bool showCurrentPickInfo = false;
        [ObservableProperty] private string currentPickInfoTitle = "";
        [ObservableProperty] private string currentPickInfoText = "";

        // Filters
        [ObservableProperty] private string selectedPositionFilter = "All";
        [ObservableProperty] private ObservableCollection<string> positionFilters = [];

        // Collections
        [ObservableProperty] private ObservableCollection<DraftPickItem> draftOrder = [];
        [ObservableProperty] private ObservableCollection<ProspectItem> allProspects = [];
        [ObservableProperty] private ObservableCollection<ProspectItem> filteredProspects = [];
        [ObservableProperty] private ObservableCollection<FranchiseItem> availableFranchises = [];
        [ObservableProperty] private ObservableCollection<FranchiseItem> selectedFranchises = [];

        // Data
        private DraftDto? _draftData;
        private List<DraftPickInfo> _picks = [];
        private List<ProspectDto> _prospects = [];
        private Dictionary<int, string> _franchiseNames = [];
        private HashSet<int> _userControlledTeams = [];
        private string _draftMethod = "manual";
        private Timer? _simulationTimer;

        public DraftViewModel(TeamService teamService)
        {
            _teamService = teamService;
            InitializePositionFilters();
            LoadAvailableFranchises();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("teamId") && query["teamId"] is int tId)
                TeamId = tId;

            // Check if returning from trade
            if (query.ContainsKey("tradedFranchiseId") && query.ContainsKey("currentPick"))
            {
                HandleTradeReturn(query);
                return;
            }

            _ = LoadViewCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            try
            {
                TeamId = teamId;
                UpdateLoadingState(true, "Loading draft data...");

                _draftData = await _teamService.GetTeamDraftAsync(TeamId);

                if (_draftData == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load draft data", "OK");
                    return;
                }

                TeamName = $"{_draftData.Location} {_draftData.Mascot}"  ?? "Unknown Team";
                _picks = ConvertPicksToPickInfo(_draftData.Picks);

                await LoadProspects();
                LoadFranchiseNames();

                // Check if draft is already in progress
                if (CurrentPickIndex > 0)
                {
                    await ContinueDraft();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load draft: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        [RelayCommand]
        private async Task StartDraft()
        {
            try
            {
                UpdateLoadingState(true, "Starting draft...");

                // Set draft method
                _draftMethod = IsManualDraft ? "manual" : (IsAutoDraft ? "auto" : "multiple");

                // Set user controlled teams
                _userControlledTeams.Clear();
                _userControlledTeams.Add(TeamId); // Always control own team

                if (IsMultipleDraft)
                {
                    foreach (var franchise in SelectedFranchises)
                    {
                        _userControlledTeams.Add(franchise.Id);
                    }
                }

                await InitializeDraftOrder();

                ShowDraftSettings = false;
                ShowDraftInterface = true;

                await StartDraftProcess();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to start draft: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        [RelayCommand]
        private void ToggleFranchiseSelection(FranchiseItem franchise)
        {
            if (SelectedFranchises.Contains(franchise))
            {
                SelectedFranchises.Remove(franchise);
                franchise.IsSelected = false;
            }
            else
            {
                SelectedFranchises.Add(franchise);
                franchise.IsSelected = true;
            }

            franchise.UpdateSelectionColors();
        }

        [RelayCommand]
        private async Task SelectPick(DraftPickItem pick)
        {
            if (pick.PickIndex == CurrentPickIndex && pick.IsUserControlled && !IsSimulating)
            {
                // This is the current pick and user controlled - allow selection
                return;
            }

            // Could add trade functionality here for future picks
        }

        [RelayCommand]
        private async Task SelectProspect(ProspectItem prospect)
        {
            if (IsSimulating || CurrentPickIndex >= DraftOrder.Count)
                return;

            var currentPick = DraftOrder[CurrentPickIndex];
            if (!currentPick.IsUserControlled)
                return;

            // Show confirmation dialog
            var confirm = await Shell.Current.DisplayAlert(
                "Confirm Selection",
                $"Draft {prospect.Name} ({prospect.Position}) with pick #{currentPick.PickNumber}?",
                "Draft Player", "Cancel");

            if (confirm)
            {
                await MakeSelection(prospect);
            }
        }

        [RelayCommand]
        private void TogglePause()
        {
            IsPaused = !IsPaused;
            PauseResumeText = IsPaused ? "Resume" : "Pause";

            if (IsPaused)
            {
                StopSimulation();
            }
            else if (_draftMethod != "manual")
            {
                StartSimulation();
            }
        }

        [RelayCommand]
        private async Task TradePick()
        {
            if (CurrentPickIndex >= DraftOrder.Count)
                return;

            var currentPick = DraftOrder[CurrentPickIndex];

            // Pause simulation before trading
            if (IsSimulating)
            {
                IsPaused = true;
                PauseResumeText = "Resume";
                StopSimulation();
            }

            // Navigate to trade view with current pick
            var parameters = new Dictionary<string, object>
            {
                ["teamId"] = TeamId,
                ["currentPick"] = currentPick.PickNumber
            };

            await BaseService.GoToAsync("//trade", parameters);
        }

        [RelayCommand]
        private async Task SaveDraft()
        {
            try
            {
                UpdateLoadingState(true, "Saving draft...");

                // Create draft results DTO
                var selections = new Dictionary<int, int>();
                foreach (var pick in DraftOrder.Where(p => p.HasSelection))
                {
                    selections[pick.PickNumber] = pick.SelectedProspect?.Id ?? 0;
                }

                var success = false;
                // Update draft data
                if (_draftData != null)
                {
                    _draftData.Selections = selections;
                    success = await _teamService.SaveDraftAsync(_draftData);
                }

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Draft saved successfully!", "OK");
                    await GoToRoster();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save draft", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save draft: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        private async Task GoToRoster() => await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = TeamId });
        
        [RelayCommand]
        private async Task Return()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Exit Draft",
                "Are you sure you want to exit? Unsaved progress will be lost.",
                "Exit", "Stay");

            if (confirm)
            {
                StopSimulation();
                await GoToRoster();
            }
        }

        [RelayCommand]
        private async Task ShowMenu()
        {
            var options = new List<string> { "Cancel" };

            if (ShowPauseButton)
                options.Insert(0, PauseResumeText);

            if (CanTrade)
                options.Insert(0, "Trade Pick");

            if (CanSaveDraft)
                options.Insert(0, "Save Draft");

            var result = await Shell.Current.DisplayActionSheet("Draft Options", "Cancel", null, options.ToArray());

            switch (result)
            {
                case "Save Draft":
                    await SaveDraft();
                    break;
                case "Trade Pick":
                    await TradePick();
                    break;
                case "Pause":
                case "Resume":
                    TogglePause();
                    break;
            }
        }

        partial void OnSelectedPositionFilterChanged(string value)
        {
            FilterProspects();
        }

        private async Task LoadProspects()
        {
            try
            {
                var prospects = await _teamService.GetDraftProspectsAsync();
                _prospects = prospects?.ToList() ?? new List<ProspectDto>();

                AllProspects.Clear();
                foreach (var prospect in _prospects)
                {
                    AllProspects.Add(new ProspectItem(prospect));
                }

                FilterProspects();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load prospects: {ex.Message}", "OK");
            }
        }

        private void FilterProspects()
        {
            FilteredProspects.Clear();

            var filtered = AllProspects.AsEnumerable();

            if (SelectedPositionFilter != "All")
            {
                filtered = filtered.Where(p => p.Position == SelectedPositionFilter);
            }

            // Filter out already drafted players
            var draftedPlayerIds = DraftOrder.Where(p => p.HasSelection)
                                            .Select(p => p.SelectedProspect?.Id ?? 0)
                                            .Where(id => id > 0)
                                            .ToHashSet();

            foreach (var prospect in filtered.OrderBy(p => p.Consensus))
            {
                FilteredProspects.Add(prospect);
            }
        }

        private async Task InitializeDraftOrder()
        {
            DraftOrder.Clear();

            // Sort picks by overall number
            var sortedPicks = _picks.OrderBy(p => p.Overall).ToList();

            for (int i = 0; i < sortedPicks.Count; i++)
            {
                var pick = sortedPicks[i];
                var isUserControlled = _userControlledTeams.Contains(pick.TeamId);

                var draftPickItem = new DraftPickItem
                {
                    PickIndex = i,
                    PickNumber = pick.Overall,
                    Round = pick.Round,
                    PickInRound = pick.PickInRound,
                    TeamId = pick.TeamId,
                    TeamName = _franchiseNames.GetValueOrDefault(pick.TeamId, "Unknown"),
                    TeamAbbreviation = GetTeamAbbreviation(pick.TeamId),
                    IsUserControlled = isUserControlled,
                    ControlText = isUserControlled ? "Your Control" : "",
                    RoundPickText = $"R{pick.Round}, P{pick.PickInRound}",
                    IsCurrentPick = false,
                    HasSelection = false
                };

                draftPickItem.UpdateColors();
                DraftOrder.Add(draftPickItem);
            }
        }

        private async Task StartDraftProcess()
        {
            CurrentPickIndex = 0;
            UpdateCurrentPick();

            // Start simulation if not manual draft
            if (_draftMethod != "manual" && !IsPaused)
            {
                StartSimulation();
            }
        }

        private async Task ContinueDraft()
        {
            // Load existing draft progress
            ShowDraftSettings = false;
            ShowDraftInterface = true;

            await InitializeDraftOrder();

            // Find current pick index based on existing selections
            // This would need to be implemented based on your draft data structure
            CurrentPickIndex = DraftOrder.Count(p => p.HasSelection);

            UpdateCurrentPick();
        }

        private void StartSimulation()
        {
            if (IsSimulating || IsPaused) return;

            IsSimulating = true;
            ShowPauseButton = true;

            _simulationTimer = new Timer(async _ => await ProcessNextPick(), null, 2000, 3000);
        }

        private void StopSimulation()
        {
            IsSimulating = false;
            _simulationTimer?.Dispose();
            _simulationTimer = null;
        }

        private async Task ProcessNextPick()
        {
            if (IsPaused || CurrentPickIndex >= DraftOrder.Count)
            {
                StopSimulation();
                return;
            }

            var currentPick = DraftOrder[CurrentPickIndex];

            if (currentPick.IsUserControlled)
            {
                // Wait for user selection
                StopSimulation();
                ShowCurrentPickInfo = true;
                CurrentPickInfoTitle = "Your Turn";
                CurrentPickInfoText = $"Select a player for pick #{currentPick.PickNumber}";
                CanTrade = true;
                return;
            }

            // Auto-select for CPU teams
            await MakeAutoPick();
        }

        private async Task MakeAutoPick()
        {
            // Simple auto-pick logic - select best available player
            var bestAvailable = FilteredProspects.OrderBy(p => p.Consensus).FirstOrDefault();

            if (bestAvailable != null)
            {
                await MakeSelection(bestAvailable);
            }
            else
            {
                // No prospects available - advance pick
                await AdvanceToNextPick();
            }
        }

        private async Task MakeSelection(ProspectItem prospect)
        {
            if (CurrentPickIndex >= DraftOrder.Count) return;

            var currentPick = DraftOrder[CurrentPickIndex];

            // Update pick with selection
            currentPick.SelectedProspect = prospect;
            currentPick.SelectedPlayerName = prospect.Name;
            currentPick.SelectedPlayerPosition = prospect.Position;
            currentPick.HasSelection = true;
            currentPick.UpdateColors();

            // Remove prospect from available list
            AllProspects.Remove(prospect);
            FilterProspects();

            await AdvanceToNextPick();
        }

        private async Task AdvanceToNextPick()
        {
            CurrentPickIndex++;

            if (CurrentPickIndex >= DraftOrder.Count)
            {
                // Draft completed
                await CompleteDraft();
                return;
            }

            UpdateCurrentPick();

            // Continue simulation if not paused and not user controlled
            var nextPick = DraftOrder[CurrentPickIndex];
            if (!nextPick.IsUserControlled && !IsPaused && _draftMethod != "manual")
            {
                if (!IsSimulating)
                    StartSimulation();
            }
            else
            {
                StopSimulation();
                if (nextPick.IsUserControlled)
                {
                    ShowCurrentPickInfo = true;
                    CurrentPickInfoTitle = "Your Turn";
                    CurrentPickInfoText = $"Select a player for pick #{nextPick.PickNumber}";
                    CanTrade = true;
                }
            }
        }

        private void UpdateCurrentPick()
        {
            // Update previous pick styling
            foreach (var pick in DraftOrder)
            {
                pick.IsCurrentPick = false;
                pick.UpdateColors();
            }

            if (CurrentPickIndex < DraftOrder.Count)
            {
                var currentPick = DraftOrder[CurrentPickIndex];
                currentPick.IsCurrentPick = true;
                currentPick.UpdateColors();

                CurrentPickText = $"Pick #{currentPick.PickNumber} - {currentPick.TeamAbbreviation}";
                ShowCurrentPickInfo = currentPick.IsUserControlled;
                CanTrade = currentPick.IsUserControlled;
            }
            else
            {
                CurrentPickText = "Draft Complete";
                ShowCurrentPickInfo = false;
                CanTrade = false;
            }

            CanSaveDraft = DraftOrder.Any(p => p.HasSelection);
        }

        private async Task CompleteDraft()
        {
            StopSimulation();
            ShowCurrentPickInfo = false;
            CanTrade = false;
            CanSaveDraft = true;
            CurrentPickText = "Draft Complete";

            await Shell.Current.DisplayAlert("Draft Complete",
                "The draft has been completed! Save your results to continue.", "OK");
        }

        private void HandleTradeReturn(IDictionary<string, object> query)
        {
            // Handle return from trade view
            if (query.ContainsKey("userPicksSent") && query["userPicksSent"] is List<int> userPicks && userPicks.Any())
            {
                // User traded away picks - update draft order
                UpdateDraftOrderAfterTrade(userPicks);
            }

            if (query.ContainsKey("franchisePicksSent") && query["franchisePicksSent"] is List<int> franchisePicks && franchisePicks.Any())
            {
                // User received picks - update draft order
                UpdateDraftOrderAfterTrade(franchisePicks, true);
            }

            // Resume draft if it was running
            UpdateCurrentPick();
        }

        private void UpdateDraftOrderAfterTrade(List<int> picks, bool received = false)
        {
            // This would need more complex logic to properly update the draft order
            // For now, just mark that a trade occurred and may need draft order refresh
            _ = LoadViewAsync(TeamId); // Reload draft data
        }

        private void LoadFranchiseNames()
        {
            _franchiseNames.Clear();
            var franchises = FranchiseInfo.GetAllFranchises();

            foreach (var franchise in franchises)
            {
                _franchiseNames[franchise.Id] = franchise.Name;
            }
        }

        private string GetTeamAbbreviation(int teamId)
        {
            if (teamId == TeamId)
                return "YOU";

            var franchise = FranchiseInfo.GetAllFranchises().FirstOrDefault(f => f.Id == teamId);
            return franchise?.Abbreviation ?? "UNK";
        }

        private void LoadAvailableFranchises()
        {
            AvailableFranchises.Clear();
            var franchises = FranchiseInfo.GetAllFranchises().Where(f => f.Id != TeamId);

            foreach (var franchise in franchises)
            {
                AvailableFranchises.Add(new FranchiseItem(franchise));
            }
        }

        private void InitializePositionFilters()
        {
            PositionFilters.Clear();
            PositionFilters.Add("All");
            PositionFilters.Add("QB");
            PositionFilters.Add("RB");
            PositionFilters.Add("WR");
            PositionFilters.Add("TE");
            PositionFilters.Add("OL");
            PositionFilters.Add("DL");
            PositionFilters.Add("LB");
            PositionFilters.Add("DB");
            PositionFilters.Add("P/K");
            PositionFilters.Add("PR/KR");
            PositionFilters.Add("LS");
        }

        private void UpdateLoadingState(bool loading, string message = "Loading...")
        {
            IsLoading = loading;
            LoadingMessage = message;
        }

        private List<DraftPickInfo> ConvertPicksToPickInfo(IList<IList<int>>? picks)
        {
            var pickInfoList = new List<DraftPickInfo>();

            if (picks == null) return pickInfoList;

            for (int teamIndex = 0; teamIndex < picks.Count; teamIndex++)
            {
                var teamPicks = picks[teamIndex];
                if (teamPicks == null) continue;

                foreach (var pick in teamPicks)
                {
                    var overall = DraftPicks.GetPickOverall(pick);
                    var round = pick / 100;
                    var pickInRound = pick % 100;

                    pickInfoList.Add(new DraftPickInfo
                    {
                        Overall = overall,
                        Round = round,
                        PickInRound = pickInRound,
                        RppFormat = pick,
                        TeamId = teamIndex,
                        IsUserTeam = teamIndex == TeamId,
                        TeamName = TeamName,
                        TeamAbbr = GetTeamAbbreviation(teamIndex)
                    });
                }
            }

            return pickInfoList.OrderBy(p => p.Overall).ToList();
        }
    }

    // Supporting classes
    public partial class DraftPickItem : ObservableObject
    {
        public int PickIndex { get; set; }
        public int PickNumber { get; set; }
        public int Round { get; set; }
        public int PickInRound { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = "";
        public string TeamAbbreviation { get; set; } = "";
        public string RoundPickText { get; set; } = "";
        public bool IsUserControlled { get; set; }
        public string ControlText { get; set; } = "";

        [ObservableProperty] private bool isCurrentPick;
        [ObservableProperty] private bool hasSelection;
        [ObservableProperty] private string selectedPlayerName = "";
        [ObservableProperty] private string selectedPlayerPosition = "";
        [ObservableProperty] private ProspectItem? selectedProspect;
        [ObservableProperty] private Color backgroundColor = Colors.White;
        [ObservableProperty] private Color borderColor = Color.FromArgb("#dee2e6");
        [ObservableProperty] private Color textColor = Colors.Black;
        [ObservableProperty] private int borderThickness = 1;

        public void UpdateColors()
        {
            if (IsCurrentPick)
            {
                BackgroundColor = Color.FromArgb("#007bff");
                BorderColor = Color.FromArgb("#0056b3");
                TextColor = Colors.White;
                BorderThickness = 2;
            }
            else if (HasSelection)
            {
                BackgroundColor = Color.FromArgb("#d4edda");
                BorderColor = Color.FromArgb("#28a745");
                TextColor = Colors.Black;
                BorderThickness = 1;
            }
            else if (IsUserControlled)
            {
                BackgroundColor = Colors.White;
                BorderColor = Color.FromArgb("#ffc107");
                TextColor = Colors.Black;
                BorderThickness = 3;
            }
            else
            {
                BackgroundColor = Colors.White;
                BorderColor = Color.FromArgb("#dee2e6");
                TextColor = Colors.Black;
                BorderThickness = 1;
            }
        }
    }

    public class ProspectItem
    {
        public int Id { get; }
        public string Name { get; }
        public string Position { get; }
        public string College { get; }
        public int Height { get; }
        public int Weight { get; }
        public int Consensus { get; }
        public int AthScore { get; }

        public string PositionCollege => $"{Position} - {College}";
        public string PhysicalInfo => $"{Height}\" {Weight} lbs";
        public string ConsensusText => $"#{Consensus}";

        public ProspectItem(ProspectDto prospect)
        {
            Id = prospect.Id ?? 0;
            Name = prospect.Name;
            Position = prospect.Position;
            College = prospect.College ?? "";
            Height = prospect.Height ?? 72;
            Weight = prospect.Weight ?? 220;
            Consensus = prospect.Consensus;
            AthScore = prospect.AthScore ;
        }
    }

    public partial class FranchiseItem : ObservableObject
    {
        public int Id { get; }
        public string Name { get; }
        public string Abbreviation { get; }

        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private Color selectionColor = Colors.White;
        [ObservableProperty] private Color selectionBorderColor = Color.FromArgb("#dee2e6");

        public FranchiseItem(FranchiseInfo franchise)
        {
            Id = franchise.Id;
            Name = franchise.Name;
            Abbreviation = franchise.Abbreviation;
        }

        public void UpdateSelectionColors()
        {
            if (IsSelected)
            {
                SelectionColor = Color.FromArgb("#e3f2fd");
                SelectionBorderColor = Color.FromArgb("#007bff");
            }
            else
            {
                SelectionColor = Colors.White;
                SelectionBorderColor = Color.FromArgb("#dee2e6");
            }
        }
    }
}