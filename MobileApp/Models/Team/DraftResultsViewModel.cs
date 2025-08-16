using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class DraftResultsViewModel : INotifyPropertyChanged
    {
        private readonly TeamService _teamService;
        private bool _isLoading;
        private bool _hasDraftResults;

        public DraftResultsViewModel(TeamService teamService, int teamId)
        {
            _teamService = teamService;
            TeamId = teamId;

            DraftResults = new ObservableCollection<DraftResultInfo>();

            LoadDraftResultsCommand = new Command(async () => await LoadDraftResultsAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public int TeamId { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool HasDraftResults
        {
            get => _hasDraftResults;
            set => SetProperty(ref _hasDraftResults, value);
        }

        public ObservableCollection<DraftResultInfo> DraftResults { get; }

        public ICommand LoadDraftResultsCommand { get; }
        public ICommand BackCommand { get; }

        // Events
        public event Func<Task> NavigateBackRequested;
        public event Func<string, string, string, Task> ShowAlertRequested;

        public async Task LoadDraftResultsAsync()
        {
            try
            {
                IsLoading = true;

                // Get draft results from service
                var draftData = await _teamService.GetTeamDraftAsync(TeamId);

                DraftResults.Clear();

                if (draftData?.Prospects != null && draftData.Prospects.Any())
                {
                    // Convert prospects to draft results
                    foreach (var prospect in draftData.Prospects.OrderBy(p => p.Consensus))
                    {
                        // Calculate pick information
                        var pickNumber = GetPickNumberForProspect(prospect, draftData);
                        var round = Math.Floor(pickNumber / 100.0);
                        var pickInRound = pickNumber % 100;
                        var overallPick = (int)((round - 1) * 32 + pickInRound);

                        DraftResults.Add(new DraftResultInfo
                        {
                            Round = (int)round,
                            PickNumber = (int)pickInRound,
                            OverallPick = overallPick,
                            SelectedProspect = prospect,
                            IsUserPick = true // All prospects in team's draft are user picks
                        });
                    }

                    HasDraftResults = true;
                }
                else
                {
                    HasDraftResults = false;
                }
            }
            catch (Exception ex)
            {
                await ShowAlertRequested?.Invoke("Error", $"Failed to load draft results: {ex.Message}", "OK");
                HasDraftResults = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private int GetPickNumberForProspect(ProspectDto prospect, DraftDto draftData)
        {
            // Try to find the pick number from selections
            if (draftData.Selections != null && prospect.Id.HasValue)
            {
                var selection = draftData.Selections.FirstOrDefault(s => s.Value == prospect.Id.Value);
                if (selection.Key != 0)
                {
                    return selection.Key;
                }
            }

            // If not found in selections, estimate based on consensus ranking
            // This is a fallback - in a real scenario you'd have better tracking
            var teamPicks = draftData.Picks?[0] ?? new List<int>();
            if (teamPicks.Any())
            {
                var prospectIndex = draftData.Prospects.ToList().IndexOf(prospect);
                if (prospectIndex >= 0 && prospectIndex < teamPicks.Count)
                {
                    return teamPicks.OrderBy(p => p).ToList()[prospectIndex];
                }
            }

            return 101; // Default to Round 1, Pick 1 if can't determine
        }

        private async Task BackAsync()
        {
            await NavigateBackRequested?.Invoke();
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