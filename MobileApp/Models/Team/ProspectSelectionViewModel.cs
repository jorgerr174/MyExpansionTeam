using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using METCore.DTOs.Player;

namespace MobileApp.Models.Team
{
    public class ProspectSelectionViewModel : INotifyPropertyChanged
    {
        private readonly List<ProspectDto> _allProspects;
        private string _searchText;
        private string _selectedPosition = "All Positions";
        private ProspectDto _selectedProspect;
        private bool _isSelectEnabled;

        public ProspectSelectionViewModel(List<ProspectDto> prospects)
        {
            _allProspects = prospects;
            FilteredProspects = [];
            Positions = [];

            SelectCommand = new Command<ProspectDto>(OnProspectSelected);
            ConfirmSelectionCommand = new Command(OnConfirmSelection, () => IsSelectEnabled);
            CancelCommand = new Command(OnCancel);

            SetupPositions();
            FilterProspects();
        }

        public ObservableCollection<ProspectDto> FilteredProspects { get; }
        public ObservableCollection<string> Positions { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterProspects();
            }
        }

        public string SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                SetProperty(ref _selectedPosition, value);
                FilterProspects();
            }
        }

        public ProspectDto SelectedProspect
        {
            get => _selectedProspect;
            set
            {
                SetProperty(ref _selectedProspect, value);
                IsSelectEnabled = value != null;
                OnPropertyChanged(nameof(SelectedProspectName));
                OnPropertyChanged(nameof(SelectedProspectPosition));
                OnPropertyChanged(nameof(SelectedProspectCollege));
                OnPropertyChanged(nameof(SelectedProspectRanking));
                OnPropertyChanged(nameof(SelectedProspectCombineStats));
                OnPropertyChanged(nameof(IsSelectedProspectVisible));
            }
        }

        public bool IsSelectEnabled
        {
            get => _isSelectEnabled;
            set
            {
                SetProperty(ref _isSelectEnabled, value);
                ((Command)ConfirmSelectionCommand).ChangeCanExecute();
            }
        }

        // Selected Prospect Display Properties
        public bool IsSelectedProspectVisible => SelectedProspect != null;
        public string SelectedProspectName => SelectedProspect?.Name ?? "";
        public string SelectedProspectPosition => SelectedProspect?.Position ?? "";
        public string SelectedProspectCollege => SelectedProspect?.College ?? "";
        public string SelectedProspectRanking => SelectedProspect != null ? $"Consensus Rank: #{SelectedProspect.Consensus}" : "";
        public string SelectedProspectCombineStats => ProspectSelectionViewModel.GetCombineStats(SelectedProspect);

        // Commands
        public ICommand SelectCommand { get; }
        public ICommand ConfirmSelectionCommand { get; }
        public ICommand CancelCommand { get; }

        // Events
        public event Func<ProspectDto, Task> ProspectSelected;
        public event Func<Task> SelectionCancelled;
        public event Func<ProspectDto, Task> ShowProspectDetailsRequested;

        private void SetupPositions()
        {
            var positions = _allProspects
                .Where(p => !string.IsNullOrEmpty(p.Position))
                .Select(p => p.Position)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            Positions.Add("All Positions");
            foreach (var position in positions)
            {
                Positions.Add(position);
            }
        }

        private void FilterProspects()
        {
            var searchText = SearchText?.Trim().ToLowerInvariant() ?? "";

            var filtered = _allProspects.Where(p =>
            {
                // Search filter
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                    (p.Name?.ToLowerInvariant().Contains(searchText, StringComparison.InvariantCultureIgnoreCase) == true) ||
                    (p.College?.ToLowerInvariant().Contains(searchText, StringComparison.InvariantCultureIgnoreCase) == true);

                // Position filter
                bool matchesPosition = SelectedPosition == "All Positions" ||
                    p.Position == SelectedPosition;

                return matchesSearch && matchesPosition;
            }).OrderBy(p => p.Consensus);

            FilteredProspects.Clear();
            foreach (var prospect in filtered)
            {
                FilteredProspects.Add(prospect);
            }
        }

        private void OnProspectSelected(ProspectDto prospect)
        {
            SelectedProspect = prospect;
        }

        private async void OnConfirmSelection()
        {
            if (SelectedProspect != null)
            {
                await ProspectSelected?.Invoke(SelectedProspect);
            }
        }

        private async void OnCancel()
        {
            await SelectionCancelled?.Invoke();
        }

        public async Task ShowProspectDetailsAsync(ProspectDto prospect)
        {
            await ShowProspectDetailsRequested?.Invoke(prospect);
        }

        private static string GetCombineStats(ProspectDto prospect)
        {
            if (prospect == null) return "";

            var stats = new List<string>();

            if (!string.IsNullOrEmpty(prospect.FortyYardDash))
                stats.Add($"40-Yard: {prospect.FortyYardDash}");

            if (!string.IsNullOrEmpty(prospect.BenchPress))
                stats.Add($"Bench: {prospect.BenchPress}");

            if (!string.IsNullOrEmpty(prospect.VertJump))
                stats.Add($"Vert: {prospect.VertJump}");

            if (prospect.AthScore > 0)
                stats.Add($"Athletic Score: {prospect.AthScore}");

            return string.Join(" | ", stats);
        }

        public static string GetProspectDetails(ProspectDto prospect)
        {
            var details = new List<string>
            {
                $"Position: {prospect.Position}",
                $"College: {prospect.College}",
                $"Height: {prospect.Height}\"",
                $"Weight: {prospect.Weight} lbs",
                $"Year: {prospect.Year}",
                $"Consensus Ranking: #{prospect.Consensus}",
                $"Athletic Score: {prospect.AthScore}"
            };

            // Add combine measurements
            if (!string.IsNullOrEmpty(prospect.HandSize))
                details.Add($"Hand Size: {prospect.HandSize}");

            if (!string.IsNullOrEmpty(prospect.ArmLength))
                details.Add($"Arm Length: {prospect.ArmLength}");

            if (!string.IsNullOrEmpty(prospect.Wingspan))
                details.Add($"Wingspan: {prospect.Wingspan}");

            // Add combine performance
            if (!string.IsNullOrEmpty(prospect.FortyYardDash))
                details.Add($"40-Yard Dash: {prospect.FortyYardDash}");

            if (!string.IsNullOrEmpty(prospect.BenchPress))
                details.Add($"Bench Press: {prospect.BenchPress}");

            if (!string.IsNullOrEmpty(prospect.VertJump))
                details.Add($"Vertical Jump: {prospect.VertJump}");

            if (!string.IsNullOrEmpty(prospect.BroadJump))
                details.Add($"Broad Jump: {prospect.BroadJump}");

            if (!string.IsNullOrEmpty(prospect.ThreeConeDrill))
                details.Add($"3-Cone Drill: {prospect.ThreeConeDrill}");

            if (!string.IsNullOrEmpty(prospect.TwentyYardShuttle))
                details.Add($"20-Yard Shuttle: {prospect.TwentyYardShuttle}");

            return string.Join("\n", details);
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