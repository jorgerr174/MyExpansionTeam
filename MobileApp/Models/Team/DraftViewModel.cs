using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public class DraftViewModel : INotifyPropertyChanged
    {
        private readonly TeamService _teamService;
        private DraftDto _draftData;
        private bool _isLoading;
        private bool _isDraftActive;
        private int _currentPickIndex;
        private string _teamName;
        private int _manualRounds = 3;
        private bool _enableTrading = true;
        private bool _isPaused;
        private DraftState _draftState;

        public DraftViewModel(TeamService teamService, int teamId)
        {
            _teamService = teamService;
            TeamId = teamId;

            _draftState = new DraftState();
            DraftOrder = new ObservableCollection<DraftPickInfo>();
            AvailableProspects = new ObservableCollection<ProspectDto>();

            // Commands
            StartDraftCommand = new Command(async () => await StartDraftAsync(), () => !IsLoading && !IsDraftActive);
            MakePickCommand = new Command(async () => await MakePickAsync(), () => CanMakePick);
            SimulatePickCommand = new Command(async () => await SimulatePickAsync(), () => CanSimulatePick);
            PauseResumeCommand = new Command(async () => await PauseResumeAsync());
            NewTradeCommand = new Command(async () => await NewTradeAsync());
            SaveDraftCommand = new Command(async () => await SaveDraftAsync());
            BackCommand = new Command(async () => await BackAsync());

            LoadDraftCommand = new Command(async () => await LoadDraftAsync());
        }

        public int TeamId { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsDraftActive
        {
            get => _isDraftActive;
            set
            {
                SetProperty(ref _isDraftActive, value);
                OnPropertyChanged(nameof(IsSettingsVisible));
                OnPropertyChanged(nameof(IsDraftSimulationVisible));
                ((Command)StartDraftCommand).ChangeCanExecute();
            }
        }

        public bool IsSettingsVisible => !IsDraftActive;
        public bool IsDraftSimulationVisible => IsDraftActive;

        public string TeamName
        {
            get => _teamName;
            set => SetProperty(ref _teamName, value);
        }

        public int ManualRounds
        {
            get => _manualRounds;
            set
            {
                SetProperty(ref _manualRounds, value);
                OnPropertyChanged(nameof(ControlMethodText));
                OnPropertyChanged(nameof(TeamPicksText));
            }
        }

        public bool EnableTrading
        {
            get => _enableTrading;
            set
            {
                SetProperty(ref _enableTrading, value);
                OnPropertyChanged(nameof(TradingStatusText));
            }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                SetProperty(ref _isPaused, value);
                OnPropertyChanged(nameof(PauseResumeText));
            }
        }

        public DraftPickInfo CurrentPick => _draftState.CurrentPick;

        public ObservableCollection<DraftPickInfo> DraftOrder { get; }
        public ObservableCollection<ProspectDto> AvailableProspects { get; }
        public Dictionary<int, int> Selections => _draftState.Selections;

        // Display Properties
        public string ControlMethodText =>
            ManualRounds == 0 ? "Auto-pick for your team" :
            ManualRounds == 7 ? "Manual control for all your picks" :
            $"Manual control for {ManualRounds} rounds";

        public string TradingStatusText => EnableTrading ? "Enabled" : "Disabled";
        public string PauseResumeText => IsPaused ? "Resume" : "Pause";
        public string CurrentPickText => CurrentPick != null ? $"Pick {CurrentPick.Overall}: {CurrentPick.TeamName}" : "";
        public string TeamPicksText => GetTeamPicksText();
        public string NextPickText => GetNextPickText();

        public bool CanMakePick => IsDraftActive && CurrentPick?.IsUserTeam == true && !IsPaused;
        public bool CanSimulatePick => IsDraftActive && CurrentPick?.IsUserTeam != true && !IsPaused;

        // Commands
        public ICommand LoadDraftCommand { get; }
        public ICommand StartDraftCommand { get; }
        public ICommand MakePickCommand { get; }
        public ICommand SimulatePickCommand { get; }
        public ICommand PauseResumeCommand { get; }
        public ICommand NewTradeCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand BackCommand { get; }

        // Events
        public event Func<Task<ProspectDto>> ProspectSelectionRequested;
        public event Func<Task> NavigateToTradeRequested;
        public event Func<Task> NavigateBackRequested;
        public event Func<string, string, string, Task> ShowAlertRequested;

        public async Task LoadDraftAsync()
        {
            try
            {
                IsLoading = true;
                _draftData = await _teamService.GetDraftAsync(TeamId);

                TeamName = $"{_draftData.Location} {_draftData.Mascot}";
                ManualRounds = _draftData.Rounds;

                if (_draftData.Selections != null)
                {
                    foreach (var selection in _draftData.Selections)
                    {
                        _draftState.Selections[selection.Key] = selection.Value;
                    }
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

        private async Task StartDraftAsync()
        {
            try
            {
                if (!ValidateSettings()) return;

                IsLoading = true;
                await InitializeDraftSimulation();
                IsDraftActive = true;
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

        private bool ValidateSettings()
        {
            if (ManualRounds < 0 || ManualRounds > 7)
            {
                ShowAlertRequested?.Invoke("Invalid Settings", "Manual rounds must be between 0 and 7", "OK");
                return false;
            }
            return true;
        }

        private async Task InitializeDraftSimulation()
        {
            // Build draft order
            BuildDraftOrder();

            // Load prospects
            var prospects = await _teamService.GetDraftProspectsAsync();
            AvailableProspects.Clear();
            foreach (var prospect in prospects.OrderBy(p => p.Consensus))
            {
                AvailableProspects.Add(prospect);
            }

            _draftState.CurrentPickIndex = 0;
            OnPropertyChanged(nameof(CurrentPick));
            OnPropertyChanged(nameof(CurrentPickText));
            OnPropertyChanged(nameof(CanMakePick));
            OnPropertyChanged(nameof(CanSimulatePick));
        }

        private void BuildDraftOrder()
        {
            DraftOrder.Clear();
            _draftState.DraftOrder.Clear();
            var allPicks = new List<DraftPickInfo>();

            for (int entityIndex = 0; entityIndex < _draftData.Picks.Count; entityIndex++)
            {
                var picks = _draftData.Picks[entityIndex];
                foreach (var pickNum in picks)
                {
                    var round = Math.Floor(pickNum / 100.0);
                    var pickInRound = pickNum % 100;
                    var overallPick = (int)((round - 1) * 32 + pickInRound);

                    allPicks.Add(new DraftPickInfo
                    {
                        Overall = overallPick,
                        Round = (int)round,
                        PickInRound = (int)pickInRound,
                        TeamId = entityIndex,
                        IsUserTeam = entityIndex == 0,
                        TeamName = entityIndex == 0 ? TeamName : $"Team {entityIndex}",
                        RppFormat = pickNum
                    });
                }
            }

            var sortedPicks = allPicks.OrderBy(p => p.Overall).ToList();
            foreach (var pick in sortedPicks)
            {
                DraftOrder.Add(pick);
                _draftState.DraftOrder.Add(pick);
            }
        }

        private async Task MakePickAsync()
        {
            try
            {
                var selectedProspect = await ProspectSelectionRequested?.Invoke();
                if (selectedProspect != null)
                {
                    await ProcessPick(selectedProspect);
                }
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to make pick: {ex.Message}", "OK");
            }
        }

        private async Task SimulatePickAsync()
        {
            try
            {
                if (AvailableProspects.Count == 0) return;

                var bestProspect = AvailableProspects.OrderBy(p => p.Consensus).First();
                await ProcessPick(bestProspect);

                // Continue simulation if needed
                if (ManualRounds == 0 || !IsManualControlRound())
                {
                    await Task.Delay(1000);
                    if (!_draftState.IsComplete && !IsPaused)
                    {
                        await SimulatePickAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to simulate pick: {ex.Message}", "OK");
            }
        }

        private async Task ProcessPick(ProspectDto prospect)
        {
            if (CurrentPick == null || prospect.Id == null) return;

            // Record the pick
            _draftState.RecordPick(CurrentPick.Overall, prospect.Id.Value);

            // Remove prospect from available list
            AvailableProspects.Remove(prospect);

            // Move to next pick
            _draftState.AdvanceToNextPick();
            OnPropertyChanged(nameof(CurrentPick));
            OnPropertyChanged(nameof(CurrentPickText));
            OnPropertyChanged(nameof(CanMakePick));
            OnPropertyChanged(nameof(CanSimulatePick));

            // Show confirmation for user picks
            if (CurrentPick?.IsUserTeam == true)
            {
                await ShowAlertRequested?.Invoke("Pick Made",
                    $"Selected {prospect.Name} ({prospect.Position}) with pick #{CurrentPick.Overall}", "OK");
            }

            // Check if draft is complete
            if (_draftState.IsComplete)
            {
                await CompleteDraft();
            }
        }

        private bool IsManualControlRound()
        {
            return CurrentPick != null && CurrentPick.Round <= ManualRounds;
        }

        private async Task PauseResumeAsync()
        {
            IsPaused = !IsPaused;

            if (!IsPaused && ManualRounds == 0)
            {
                await SimulatePickAsync();
            }
        }

        private async Task NewTradeAsync()
        {
            await NavigateToTradeRequested?.Invoke();
        }

        private async Task SaveDraftAsync()
        {
            try
            {
                IsLoading = true;
                var draftDto = CreateDraftDto();
                await _teamService.SaveDraftProgressAsync(draftDto);
                await ShowAlertRequested?.Invoke("Draft Saved", "Your draft progress has been saved successfully.", "OK");
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

        private async Task BackAsync()
        {
            if (IsDraftActive && HasUnsavedProgress())
            {
                // Note: This would need to be handled in the view since we can't show dialogs from ViewModel
                await NavigateBackRequested?.Invoke();
            }
            else
            {
                await NavigateBackRequested?.Invoke();
            }
        }

        private async Task CompleteDraft()
        {
            try
            {
                IsLoading = true;
                var draftDto = CreateDraftDto();
                await _teamService.SaveDraftAsync(draftDto);

                IsDraftActive = false;
                await ShowAlertRequested?.Invoke("Draft Complete",
                    "Congratulations! Your draft has been completed successfully.", "OK");
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

        private DraftDto CreateDraftDto()
        {
            return new DraftDto(
                _draftData.Id,
                _draftData.Location,
                _draftData.Abb,
                _draftData.Mascot,
                _draftData.UserUsername,
                _draftData.Date,
                _draftData.Complete,
                ManualRounds,
                _draftState.Selections
            )
            {
                Picks = _draftData.Picks,
                Prospects = _draftData.Prospects
            };
        }

        private bool HasUnsavedProgress()
        {
            return _draftState.HasUnsavedProgress;
        }

        private string GetTeamPicksText()
        {
            if (_draftData?.Picks != null && _draftData.Picks.Count > 0)
            {
                var teamPicksCount = _draftData.Picks[0]?.Count ?? 0;
                return $"Your Draft Picks: {teamPicksCount}";
            }
            return "Your Draft Picks: 0";
        }

        private string GetNextPickText()
        {
            if (IsDraftActive && DraftOrder.Count > _draftState.CurrentPickIndex)
            {
                var nextUserPick = DraftOrder.Skip(_draftState.CurrentPickIndex).FirstOrDefault(p => p.IsUserTeam);
                if (nextUserPick != null)
                {
                    return $"Next Pick: Pick {nextUserPick.Overall} (Round {nextUserPick.Round})";
                }
            }

            if (_draftData?.Picks != null && _draftData.Picks.Count > 0 && _draftData.Picks[0].Any())
            {
                var firstPick = _draftData.Picks[0].Min();
                var round = Math.Floor(firstPick / 100.0);
                var pickInRound = firstPick % 100;
                return $"Next Pick: Round {round}, Pick {pickInRound}";
            }

            return "Next Pick: No picks available";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}