using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Team
{
    public partial class DraftViewModel(TeamService teamService) : TeamBaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private string loadingMessage = "Cargando...";

        // Draft Setup
        [ObservableProperty] private bool showDraftInterface = false;
        [ObservableProperty] private bool isAutoDraft = false;
        [ObservableProperty] private bool isOnlyTeam = true;
        [ObservableProperty] private bool isMultipleDraft = false;

        // Draft State
        [ObservableProperty] private int currentPickIndex = 0;
        [ObservableProperty] private string currentPickText = "";

        [ObservableProperty] private bool isPaused = true;
        [ObservableProperty] private bool showPauseButton = true;
        [ObservableProperty] private string pauseResumeText = "Start";

        // Current Pick Info
        [ObservableProperty] private string currentPickInfoTitle = "";
        [ObservableProperty] private string currentPickInfoText = "";

        // Filters
        [ObservableProperty] private string selectedPositionFilter = "All";
        [ObservableProperty] private ObservableCollection<string> positionFilters = [];

        // Collections
        [ObservableProperty] private ObservableCollection<DraftPickItem> draftOrder = [];
        [ObservableProperty] private ObservableCollection<ProspectItem> prospects = [];
        [ObservableProperty] private ObservableCollection<FranchiseItem> franchises = [];

        public ObservableCollection<ProspectItem> FilteredProspects => new(Prospects.Where(p => p.IsVisible));

        [ObservableProperty] private int manualRounds = 3;
        [ObservableProperty] private bool allowTrades = true;
        [ObservableProperty] private bool showCurrentPickInfo = false;

        // Data
        [NotNull] private DraftDto Draft;
        [ObservableProperty] private string teamName = string.Empty;

        public bool ShowDraftSettings => CurrentPickIndex == 0;
        [ObservableProperty] private bool isDraftComplete = false;

        public DraftPickItem CurrentPick => DraftOrder[CurrentPickIndex - 1];

        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    if (query.ContainsKey("teamId") && query["teamId"] is int tId)
        //        TeamId = tId;

        //    // Check if returning from trade
        //    if (query.ContainsKey("tradedFranchiseId") && query.ContainsKey("currentPick"))
        //    {
        //        HandleTradeReturn(query);
        //        return;
        //    }

        //    _ = LoadViewCommand.ExecuteAsync(null);
        //}


        #region OnLoad
        private void LoadFranchises()
        {
            Franchises.Clear();
            foreach (FranchiseInfo franchise in FranchiseInfo.GetAllFranchises())
                Franchises.Add(new FranchiseItem(franchise));
        }

        private void InitializePositionFilters()
        {
            PositionFilters.Clear();
            PositionFilters.Add("All");
            PositionFilters.Add("QB");
            PositionFilters.Add("HB");
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

        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            try
            {
                UpdateLoadingState(true, "Cargando draft data...");

                if (await _teamService.GetTeamDraftAsync(teamId) is DraftDto draft)
                {
                    Draft = draft;
                    TeamName = $"{Draft.Location} {Draft.Mascot}";
                    OnPropertyChanged(nameof(TeamName));

                    Prospects.Clear();
                    foreach (ProspectDto prospect in await _teamService.GetDraftProspectsAsync() ?? [])
                        Prospects.Add(new ProspectItem(prospect));
                    OnPropertyChanged(nameof(FilteredProspects));
                    InitializePositionFilters();

                    LoadFranchises();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load draft data", "OK");
                    return;
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
        #endregion OnLoad


        #region DraftSettings
        private async Task ConvertPicksToPickItem(IList<IList<int>>? picks)
        {
            if (!picks.Any()) return;

            IList<DraftPickItem> list = [];
            IList<IList<DraftPickItem>> teamsPicks = [];

            int totalManualPicks = DraftPicks.TotalAtRound[IsAutoDraft ? 0 : ManualRounds];
            foreach (int pick in picks?[0] ?? [])
                list.Add(new DraftPickItem(pick, totalManualPicks, !IsAutoDraft, TeamName, Draft.Abb));
            teamsPicks.Add(list);

            FranchiseItem franchise;
            bool userControlled;
            IList<int> teamPicks;
            IList<int> selectedFranchisesIds = [.. Franchises.Where(f => f.IsSelected).Select(f => f.Id)];
            for (int i = 1; i < picks.Count; i++)
            {
                list = [];
                franchise = Franchises[i - 1];
                userControlled = selectedFranchisesIds.Contains(franchise.Id);
                teamPicks = picks[i];
                foreach (int pick in teamPicks ?? [])
                    list.Add(new DraftPickItem(pick, totalManualPicks, userControlled, franchise));
                teamsPicks.Add(list);
            }

            foreach (DraftPickItem pick in teamsPicks.SelectMany(team => team).OrderBy(p => p.Overall))
                DraftOrder.Add(pick);
        }

        [RelayCommand]
        private void ToggleFranchiseSelection(FranchiseItem franchise)
        {
            franchise.IsSelected = !franchise.IsSelected;
            franchise.UpdateSelectionColors();
        }

        [RelayCommand]
        private async Task LoadDraft()
        {
            try
            {
                UpdateLoadingState(true, "Starting draft...");
                ShowDraftInterface = true;

                await ConvertPicksToPickItem(Draft.Picks);
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
        #endregion DraftSettings


        #region Extra functionality
        private void UpdateLoadingState(bool loading, string message = "Cargando...")
        {
            IsLoading = loading;
            LoadingMessage = message;
        }

        [RelayCommand]
        private async Task TogglePause()
        {
            IsPaused = !IsPaused;
            PauseResumeText = IsPaused ? "Resume" : "Pause";

            if (!IsPaused) await AdvanceToNextPick();
        }

        private void FilterProspects()
        {
            string[] positions =
                SelectedPositionFilter switch
                {
                    "QB" => ["QB"],
                    "HB" => ["RB", "FB"],
                    "WR" => ["WR"],
                    "TE" => ["TE"],
                    "OL" => ["OT", "G", "C"],
                    "DL" => ["NT", "DT", "ED"],
                    "LB" => ["OLB", "MLB"],
                    "DB" => ["DB", "CB", "S", "FS", "SS"],
                    "P/K" => ["P", "K"],
                    "PR/KR" => ["PR", "KR"],
                    "LS" => ["LS"],
                    _ => [] //All
                };

            if (positions.Length == 0) foreach (ProspectItem prospect in Prospects) prospect.IsVisible = true;
            else if (positions.Length == 1)
            {
                string position = positions[0];
                foreach (ProspectItem prospect in Prospects) prospect.IsVisible = prospect.Position == position;
            }
            else foreach (ProspectItem prospect in Prospects) prospect.IsVisible = positions.Contains(prospect.Position);

            OnPropertyChanged(nameof(FilteredProspects));
        }

        private async Task FinishDraft()
        {
            IsDraftComplete = true;
            ShowPauseButton = false;

            AllowTrades = false;
            IsPaused = true;
            CurrentPickText = "Draft Complete";

            await Shell.Current.DisplayAlert("Draft Complete", "The draft has been completed! Save your results to continue.", "OK");
        }
        #endregion Extra functionality


        #region Flow functionality
        private async Task AdvanceToNextPick()
        {
            if (IsPaused) return;
            CurrentPickIndex++;

            if (CurrentPickIndex >= DraftOrder.Count)
            {
                await FinishDraft();
                return;
            }

            UpdatePickDisplays();

            if (!CurrentPick.IsUserControlled)
            {
                CurrentPickInfoTitle = $"{CurrentPick.TeamAbb} Turn";
                CurrentPickInfoText = $"Analyzing prospects for pick #{CurrentPick.Overall}";

                await Task.Delay(500);
                await MakeSelection(Prospects.OrderBy(p => p.Consensus).FirstOrDefault());
            }
            else
            {
                CurrentPickInfoTitle = "Your Turn";
                CurrentPickInfoText = $"Select a player for pick #{CurrentPick.Overall}";
                ShowPauseButton = false;
            }
        }

        [RelayCommand]
        private async Task SelectProspect(ProspectItem prospect)
        {
            if (CurrentPickIndex <= DraftOrder.Count && CurrentPick.IsUserControlled &&
                await Shell.Current.DisplayAlert("Confirm Selection", $"Draft {prospect.Name} ({prospect.Position}) with pick #{CurrentPick.Overall}?", "Draft Player", "Cancel"))
            {
                ShowPauseButton = true;
                await MakeSelection(prospect);
            }
        }

        private async Task MakeSelection(ProspectItem prospect)
        {
            CurrentPick.AssignProspect(prospect);
            Prospects.Remove(prospect);
            OnPropertyChanged(nameof(FilteredProspects));
            FilterProspects();

            await Task.Delay(1000);
            await AdvanceToNextPick();
        }

        private void UpdatePickDisplays()
        {
            try
            {
                if (DraftOrder[CurrentPickIndex - 2] is DraftPickItem previous)
                    previous.UpdateColors();
            }
            catch { }

            try
            {
                CurrentPickText = $"Pick #{CurrentPick.Overall} - {CurrentPick.TeamAbb}";
                CurrentPick.UpdateColorsCurrentPick();
            }
            catch { }
        }
        #endregion Flow functionality


        #region Trade
        [RelayCommand]
        private async Task Trade()
        {
            if (IsDraftComplete) return;

            IsPaused = true;
            PauseResumeText = "Resume";
            await BaseService.GoToAsync(AppRoutes.Trade, new() { ["teamId"] = Draft.Id, ["currentPick"] = CurrentPick.Overall });
        }

        public void HandleTradeReturn(int franchiseId, IList<int> teamPicks, IList<int> franchisePicks)
        {
            FranchiseItem? franchise = Franchises[franchiseId - 1];

            bool isFranchiseControlled = franchise.IsSelected;
            bool isTeamControlled = !IsAutoDraft;
            DraftPickItem pickItem;

            foreach (int pick in franchisePicks ?? [])
            {
                pickItem = DraftOrder[DraftPicks.GetPickOverall(pick)];
                pickItem.UpdateOwnerAfterTrade(isFranchiseControlled, franchise, null, null);
                OnPropertyChanged(nameof(pickItem));
            }

            foreach (int pick in franchisePicks ?? [])
            {
                pickItem = DraftOrder[DraftPicks.GetPickOverall(pick)];
                pickItem.UpdateOwnerAfterTrade(isFranchiseControlled, franchise, TeamName, Draft.Abb);
                OnPropertyChanged(nameof(pickItem));
            }
        }
        #endregion Trade


        #region End actions
        [RelayCommand]
        private async Task SaveDraft()
        {
            try
            {
                UpdateLoadingState(true, "Saving draft...");

                Dictionary<int, int> selections = [];
                foreach (var pick in DraftOrder.Where(p => p.IsUserControlled && p.HasSelection))
                    selections[pick.Overall] = pick.Prospect?.Id ?? 0;

                var success = false;
                if (Draft != null)
                {
                    Draft.Selections = selections;
                    success = await _teamService.SaveDraftAsync(Draft);
                }

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Draft saved successfully!", "OK");
                    await GoToRoster();
                }
                else
                    await Shell.Current.DisplayAlert("Error", "Failed to save draft", "OK");
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

        private async Task GoToRoster() => await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = Draft.Id });

        [RelayCommand]
        private async Task Return()
        {
            if (await Shell.Current.DisplayAlert("Exit Draft", "Are you sure you want to exit? Unsaved progress will be lost.", "Exit", "Stay"))
            {
                IsPaused = true;
                await GoToRoster();
            }
        }
        #endregion End actions
    }

    public partial class DraftPickItem : ObservableObject
    {
        public FranchiseItem? Franchise { get; private set; }

        public int RppFormat { get; private set; }
        public int Overall { get; private set; }
        public int Round { get; private set; }
        public int PickInRound { get; private set; }

        public int TeamId { get; private set; }
        public string TeamName { get; private set; }
        public string TeamAbb { get; private set; }

        public bool IsUserControlled { get; private set; }
        public string ControlText { get; set; } = "";

        public string Pickround => $"Round: {this.Round} PicK: {this.PickInRound}";
        private bool isCurrentPick(int currentPick) => currentPick == Overall;
        public bool HasSelection => Prospect is ProspectItem;
        private string SelectedPlayerName => Prospect?.Name ?? string.Empty;
        private string SelectedPlayerPosition => Prospect?.Position ?? string.Empty;

        [ObservableProperty] private Color backgroundColor = Colors.White;
        [ObservableProperty] private Color borderColor = Color.FromArgb("#dee2e6");
        [ObservableProperty] private Color textColor = Colors.Black;
        [ObservableProperty] private int borderThickness = 1;
        [ObservableProperty] private ProspectItem? prospect;

        private DraftPickItem(int pick)
        {
            this.RppFormat = pick;
            this.Overall = DraftPicks.GetPickOverall(pick);
            this.Round = pick / 100;
            this.PickInRound = pick % 100;
        }

        public DraftPickItem(int pick, int totalManualPicks, bool isUserControlled, string teamName, string teamAbb) : this(pick)
        {
            this.IsUserControlled = this.Overall <= totalManualPicks && isUserControlled;
            this.Franchise = null;
            this.TeamId = 0;
            this.TeamName = teamName;
            this.TeamAbb = teamAbb;

            this.UpdateColors();
        }

        public DraftPickItem(int pick, int totalManualPicks, bool isUserControlled, FranchiseItem franchise) : this(pick)
        {
            this.IsUserControlled = this.Overall <= totalManualPicks && isUserControlled;
            this.Franchise = franchise;
            this.TeamId = franchise.Id;
            this.TeamName = franchise.Name;
            this.TeamAbb = franchise.Abb;

            this.UpdateColors();
        }

        public DraftPickItem(int pick, int totalManualPicks, bool isUserControlled, FranchiseItem? franchise, string teamName, string teamAbb) : this(pick)
        {
            this.IsUserControlled = this.Overall <= totalManualPicks && isUserControlled;
            this.Franchise = franchise;
            this.TeamId = Franchise is null ? 0 : franchise.Id;
            this.TeamName = Franchise is null ? teamName : franchise.Name;
            this.TeamAbb = Franchise is null ? teamAbb : franchise.Abb;

            this.UpdateColors();
        }

        public void UpdateColorsCurrentPick()
        {
            BackgroundColor = Color.FromArgb("#007bff");
            BorderColor = Color.FromArgb("#0056b3");
            TextColor = Colors.White;
            BorderThickness = 2;
        }
        public void UpdateColors()
        {
            if (HasSelection)
            {
                BackgroundColor = Color.FromArgb("#d4edda");
                BorderColor = Color.FromArgb("#28a745");
                TextColor = Colors.Black;
                BorderThickness = 1;
            }
            else if (Franchise is null)
            {
                BackgroundColor = Color.FromArgb("#91C4FF");
                BorderColor = Color.FromArgb("#ffc107");
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
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(BorderThickness));
        }
        public void UpdateOwnerAfterTrade(bool isUserControlled, FranchiseItem? franchise, string? teamName, string? teamAbb)
        {
            this.IsUserControlled = isUserControlled;
            this.Franchise = franchise;
            this.TeamId = Franchise is null ? 0 : franchise.Id;
            this.TeamName = Franchise is null ? teamName : franchise.Name;
            this.TeamAbb = Franchise is null ? teamAbb : franchise.Abb;

            OnPropertyChanged(nameof(IsUserControlled));
            OnPropertyChanged(nameof(Franchise));
            OnPropertyChanged(nameof(TeamId));
            OnPropertyChanged(nameof(TeamName));
            OnPropertyChanged(nameof(TeamAbb));
        }
        public void AssignProspect(ProspectItem prospect)
        {
            this.Prospect = prospect;
            OnPropertyChanged(nameof(Prospect));
            OnPropertyChanged(nameof(Prospect.Name));
            OnPropertyChanged(nameof(Prospect.Position));
        }
    }

    public partial class ProspectItem(ProspectDto prospect) : ObservableObject
    {
        [ObservableProperty] private bool isVisible = true;
        public ProspectDto Prospect { get; } = prospect;
        public int Id => Prospect.Id ?? 0;
        public string Name => Prospect.Name;
        public string Position => Prospect.Position;
        public int Consensus => Prospect.Consensus;


        public string PositionCollege => $"{Prospect.Position} - {Prospect.College}";
        public string PhysicalInfo => $"{Prospect.Height}\" {Prospect.Weight} lbs";
        public string ConsensusText => $"#{Prospect.Consensus}";
    }

    public partial class FranchiseItem(FranchiseInfo franchise) : ObservableObject
    {
        public int Id { get; } = franchise.Id;
        public string Name { get; } = franchise.Name;
        public string Abb { get; } = franchise.Abbreviation;

        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private Color selectionColor = Colors.White;
        [ObservableProperty] private Color selectionBorderColor = Color.FromArgb("#dee2e6");

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