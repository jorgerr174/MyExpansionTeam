using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class DraftViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        private readonly DraftState _draftState = new();
        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private bool isDraftActive = false;
        [ObservableProperty] private bool showDraftSettings = true;

        // Draft Settings
        [ObservableProperty] private string selectedDraftMethod = "full";
        [ObservableProperty] private int manualRounds = 0;
        [ObservableProperty] private bool showFranchiseSelection = false;

        // Draft State
        [ObservableProperty] private bool isPaused = true;
        [ObservableProperty] private string currentPickText = "Draft not started";
        [ObservableProperty] private string teamPicksText = "Your Draft Picks: 0";
        [ObservableProperty] private string nextPickText = "Next Pick: No picks available";
        [ObservableProperty] private string pauseResumeText = "Resume";

        // Collections
        public ObservableCollection<DraftPickInfo> DraftOrder { get; } = [];
        public ObservableCollection<ProspectDto> AvailableProspects { get; } = [];
        public ObservableCollection<FranchiseSelectionItem> SelectedFranchises { get; } = [];

        // Current pick info
        public DraftPickInfo? CurrentPick => _draftState.CurrentPick;
        public bool CanMakePick => IsDraftActive && !IsPaused && _draftState.IsCurrentPickUserControlled;
        public bool CanSimulatePick => IsDraftActive && !IsPaused && !_draftState.IsCurrentPickUserControlled;
        public bool CanPauseResume => IsDraftActive && !_draftState.IsComplete;

        public List<string> DraftMethods => ["full", "myteam", "multiple"];
        public List<int> RoundOptions => [.. Enumerable.Range(0, 8)];
        public bool CanConfigureFranchises => SelectedDraftMethod == "multiple";

        [RelayCommand]
        public async Task LoadDraftAsync()
        {
            try
            {
                IsLoading = true;
                if (await _teamService.GetTeamDraftAsync(TeamId) is DraftDto draftData)
                {
                    _draftState.TeamId = TeamId;
                    _draftState.OriginalDraftData = draftData;
                    TeamName = $"{draftData.Location} {draftData.Mascot}";

                    // Load available prospects
                    var prospects = await GetDraftProspectsAsync();
                    _draftState.AvailableProspects = [.. prospects];

                    // Update UI
                    UpdateAvailableProspects();
                    UpdateDraftInfo();
                    InitializeFranchiseSelection();

                    // If draft was previously started, restore state
                    if (draftData.Selections.Any())
                        RestoreDraftProgress(draftData.Selections);
                }
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to load draft: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task StartDraftAsync()
        {
            try
            {
                if (!ValidateSettings()) return;

                IsLoading = true;

                // Configure draft state
                _draftState.DraftMethod = SelectedDraftMethod;
                _draftState.ManualRounds = ManualRounds;
                _draftState.SelectedFranchises = [.. SelectedFranchises.Where(f => f.IsSelected).Select(f => f.FranchiseId)];

                // Build draft order
                _draftState.BuildDraftOrder();

                // Update UI collections
                UpdateDraftOrder();
                UpdateAvailableProspects();

                // Start draft
                IsDraftActive = true;
                ShowDraftSettings = false;
                IsPaused = SelectedDraftMethod != "full";

                UpdateDraftInfo();

                // Begin simulation if not paused
                if (!IsPaused)
                    await ProcessNextPickAsync();
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to start draft: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task MakePickAsync()
        {
            try
            {
                if (!CanMakePick) return;

                if (await ProspectSelectionRequested?.Invoke() is ProspectDto selectedProspect)
                    await ProcessPickSelectionAsync(selectedProspect);
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to make pick: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SimulatePickAsync()
        {
            try
            {
                if (!CanSimulatePick) return;

                if (SelectBestAvailableProspect() is ProspectDto bestProspect)
                    await ProcessPickSelectionAsync(bestProspect);
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to simulate pick: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task PauseResumeAsync()
        {
            IsPaused = !IsPaused;
            PauseResumeText = IsPaused ? "Resume" : "Pause";

            if (!IsPaused && SelectedDraftMethod == "full")
                await ProcessNextPickAsync();
        }

        public void OnDraftMethodChanged()
        {
            ShowFranchiseSelection = SelectedDraftMethod == "multiple";
            OnPropertyChanged(nameof(CanConfigureFranchises));
        }

        [RelayCommand]
        public async Task SaveDraftAsync()
        {
            try
            {
                IsLoading = true;

                if (await _teamService.SaveDraftAsync(_draftState.CreateDraftDto()))
                    await ShowAlertRequested?.Invoke("Draft Saved", "Your draft progress has been saved successfully.", "OK");
                else
                    await ShowAlertRequested?.Invoke("Error", "Failed to save draft", "OK");
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to save draft: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand] public async Task GoBackAsync() => _teamService.GoBackAsync(null);

        private async Task<IEnumerable<ProspectDto>> GetDraftProspectsAsync()
            => await _teamService.GetDraftProspectsAsync(DateTime.Now.Year);

        private bool ValidateSettings()
        {
            if (ManualRounds < 0 || ManualRounds > 7)
            {
                ShowAlertRequested?.Invoke("Invalid Settings", "Manual rounds must be between 0 and 7", "OK");
                return false;
            }

            if (SelectedDraftMethod == "multiple" && !SelectedFranchises.Any(f => f.IsSelected))
            {
                ShowAlertRequested?.Invoke("Invalid Settings", "Please select at least one franchise to control", "OK");
                return false;
            }

            return true;
        }

        private async Task ProcessPickSelectionAsync(ProspectDto prospect)
        {
            if (CurrentPick == null || prospect.Id == null) return;

            // Record the pick
            _draftState.RecordPick(CurrentPick.RppFormat, prospect.Id.Value);
            CurrentPick.SelectedPlayer = prospect;

            // Update UI
            UpdateAvailableProspects();

            // Show confirmation for user picks
            if (_draftState.IsCurrentPickUserControlled)
                await ShowAlertRequested?.Invoke("Pick Made", $"Selected {prospect.Name} ({prospect.Position}) with pick #{CurrentPick.Overall}", "OK");

            // Move to next pick
            _draftState.AdvanceToNextPick();
            UpdateDraftInfo();

            // Check if draft is complete
            if (_draftState.IsComplete)
            {
                await CompleteDraftAsync();
                return;
            }

            // Continue with next pick
            await ProcessNextPickAsync();
        }

        private async Task ProcessNextPickAsync()
        {
            if (_draftState.IsComplete || IsPaused) return;

            // If current pick is user controlled, wait for user action
            if (_draftState.IsCurrentPickUserControlled)
            {
                UpdateDraftInfo();
                return;
            }

            // Auto-simulate this pick
            await Task.Delay(1000);
            await SimulatePickAsync();
        }

        private ProspectDto? SelectBestAvailableProspect()
        {
            if (!AvailableProspects.Any()) return null;
            return AvailableProspects.OrderBy(p => p.Consensus).FirstOrDefault();
        }

        private async Task CompleteDraftAsync()
        {
            try
            {
                IsLoading = true;
                await _teamService.SaveDraftAsync(_draftState.CreateDraftDto());

                IsDraftActive = false;
                await ShowAlertRequested?.Invoke("Draft Complete", "Congratulations! Your draft has been completed successfully.", "OK");
                await NavigateBackRequested?.Invoke();
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to complete draft: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateDraftOrder()
        {
            DraftOrder.Clear();
            foreach (var pick in _draftState.DraftOrder)
                DraftOrder.Add(pick);
        }

        private void UpdateAvailableProspects()
        {
            AvailableProspects.Clear();
            foreach (var prospect in _draftState.AvailableProspects)
                AvailableProspects.Add(prospect);
        }

        private void UpdateDraftInfo()
        {
            CurrentPickText = CurrentPick != null ? $"Pick #{CurrentPick.Overall} - {CurrentPick.TeamAbbr}" : "Draft Complete";

            TeamPicksText = GetTeamPicksText();
            NextPickText = GetNextPickText();

            OnPropertyChanged(nameof(CurrentPick));
            OnPropertyChanged(nameof(CanMakePick));
            OnPropertyChanged(nameof(CanSimulatePick));
            OnPropertyChanged(nameof(CanPauseResume));
        }

        private string GetTeamPicksText()
            => _draftState.OriginalDraftData?.Picks != null && _draftState.OriginalDraftData.Picks.Count > 0
                ? $"Your Draft Picks: {_draftState.OriginalDraftData.Picks[0]?.Count ?? 0}"
                : "Your Draft Picks: 0";

        private string GetNextPickText()
        {
            if (IsDraftActive && _draftState.DraftOrder.Count > _draftState.CurrentPickIndex)
            {
                var nextUserPick = _draftState.DraftOrder
                    .Skip(_draftState.CurrentPickIndex)
                    .FirstOrDefault(p => _draftState.IsPickUserControlled(p));

                if (nextUserPick != null)
                    return $"Next Pick: Pick {nextUserPick.Overall} (Round {nextUserPick.Round})";
            }

            return "Next Pick: No user picks remaining";
        }

        private void InitializeFranchiseSelection()
        {
            SelectedFranchises.Clear();

            foreach (var franchise in FranchiseHelper.GetAllFranchises())
                SelectedFranchises.Add(new FranchiseSelectionItem
                {
                    FranchiseId = franchise.Key,
                    Name = franchise.Value.Name,
                    Abbreviation = franchise.Value.Abbreviation,
                    IsSelected = false
                });
        }

        private void RestoreDraftProgress(IDictionary<int, int> selections)
        {
            _draftState.AllSelections = new Dictionary<int, int>(selections);
            // TODO: Restore UI state based on selections
        }

        public event Func<Task<ProspectDto?>>? ProspectSelectionRequested;
        public event Func<string, string, string, Task>? ShowAlertRequested;
        public event Func<Task>? NavigateBackRequested;
    }

    public class FranchiseSelectionItem : INotifyPropertyChanged
    {
        public int FranchiseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}