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
    public partial class TradeViewModel : TeamBaseViewModel
    {
        private readonly TeamService _teamService;

        [ObservableProperty] private int teamId;
        [ObservableProperty] private int currentPick = -1; // -1 = from roster, >=0 = from draft
        [ObservableProperty] private int selectedFranchiseId;
        [ObservableProperty] private string tradePartnerName = "Select Franchise";
        [ObservableProperty] private string loadingMessage = "Loading...";

        // Tab Management
        [ObservableProperty] private bool isSummaryTabVisible = true;
        [ObservableProperty] private bool isUserTabVisible = false;
        [ObservableProperty] private bool isFranchiseTabVisible = false;
        [ObservableProperty] private Color summaryTabColor = Color.FromArgb("#007bff");
        [ObservableProperty] private Color summaryTabTextColor = Colors.White;
        [ObservableProperty] private Color userTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color userTabTextColor = Color.FromArgb("#6c757d");
        [ObservableProperty] private Color franchiseTabColor = Color.FromArgb("#f8f9fa");
        [ObservableProperty] private Color franchiseTabTextColor = Color.FromArgb("#6c757d");

        // Trade Value Display
        [ObservableProperty] private string userTotalValueText = "Your Value: 0";
        [ObservableProperty] private string franchiseTotalValueText = "Their Value: 0";
        [ObservableProperty] private string tradeBalanceText = "Select items to trade";
        [ObservableProperty] private double userValueBarWidth = 0;
        [ObservableProperty] private double franchiseValueBarWidth = 0;

        // Trade Items
        [ObservableProperty] private ObservableCollection<TradePlayerItem> userPlayers = new();
        [ObservableProperty] private ObservableCollection<TradePickItem> userPicks = new();
        [ObservableProperty] private ObservableCollection<TradePlayerItem> franchisePlayers = new();
        [ObservableProperty] private ObservableCollection<TradePickItem> franchisePicks = new();
        [ObservableProperty] private ObservableCollection<TradeItemSummary> selectedUserItems = new();
        [ObservableProperty] private ObservableCollection<TradeItemSummary> selectedFranchiseItems = new();

        // Visibility
        [ObservableProperty] private bool hasUserPlayers = false;
        [ObservableProperty] private bool hasUserPicks = false;
        [ObservableProperty] private bool hasFranchisePlayers = false;
        [ObservableProperty] private bool hasFranchisePicks = false;
        [ObservableProperty] private bool canTrade = false;

        // Franchise Selection
        private List<FranchiseInfo> _availableFranchises = new();
        private TradeDto? _currentTradeData;
        private int _userTotalValue;
        private int _franchiseTotalValue;

        public TradeViewModel(TeamService teamService)
        {
            _teamService = teamService;
            LoadAvailableFranchises();
        }

        public void LoadTrade(int teamId, int currentPick)
        {
            SelectedFranchiseId = 0;
            CurrentPick = currentPick;
            _ = LoadViewAsync(teamId);
        }

        [RelayCommand]
        public override async Task LoadViewAsync(int teamId)
        {
            TeamId = teamId;
            try
            {
                UpdateLoadingState(true, "Loading trade interface...");

                if (SelectedFranchiseId == 0)
                    await SelectFranchise();
                else
                    await LoadTradeData();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load trade data: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        [RelayCommand]
        private async Task SelectFranchise()
        {
            try
            {
                var franchiseNames = _availableFranchises.Select(f => f.Name).ToArray();
                var selectedName = await Shell.Current.DisplayActionSheet(
                    "Select franchise to trade with", "Cancel", null, franchiseNames);

                if (selectedName != "Cancel" && !string.IsNullOrEmpty(selectedName))
                {
                    var selectedFranchise = _availableFranchises.FirstOrDefault(f => f.Name == selectedName);
                    if (selectedFranchise != null)
                    {
                        SelectedFranchiseId = selectedFranchise.Id;
                        TradePartnerName = selectedFranchise.Name;
                        await LoadTradeData();
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to select franchise: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void ShowSummaryTab()
        {
            UpdateTabVisibility("summary");
        }

        [RelayCommand]
        private void ShowUserTab()
        {
            UpdateTabVisibility("user");
        }

        [RelayCommand]
        private void ShowFranchiseTab()
        {
            UpdateTabVisibility("franchise");
        }

        [RelayCommand]
        private void TogglePlayerSelection(TradePlayerItem playerItem)
        {
            playerItem.IsSelected = !playerItem.IsSelected;
            UpdateTradeCalculations();
        }

        [RelayCommand]
        private void TogglePickSelection(TradePickItem pickItem)
        {
            pickItem.IsSelected = !pickItem.IsSelected;
            UpdateTradeCalculations();
        }

        [RelayCommand]
        private async Task ProposeTradeAsync()
        {
            await SubmitTrade(false);
        }

        [RelayCommand]
        private async Task ForceTradeAsync()
        {
            var confirm = await Shell.Current.DisplayAlert("Force Trade",
                "Force this trade even if values don't match?", "Yes", "No");
            if (confirm)
            {
                await SubmitTrade(true);
            }
        }

        [RelayCommand]
        private async Task CancelTrade()
        {
            await ReturnToOrigin();
        }

        [RelayCommand]
        private async Task Return() => await ReturnToOrigin();

        private async Task LoadTradeData()
        {
            if (SelectedFranchiseId == 0) return;

            try
            {
                UpdateLoadingState(true, $"Loading trade data with {TradePartnerName}...");

                _currentTradeData = await _teamService.GetTradeDataAsync(TeamId, SelectedFranchiseId);

                if (_currentTradeData == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load trade data", "OK");
                    return;
                }

                LoadTradeItems();
                UpdateTradeCalculations();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load trade data: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        private void LoadTradeItems()
        {
            if (_currentTradeData == null) return;

            // Clear existing items
            UserPlayers.Clear();
            UserPicks.Clear();
            FranchisePlayers.Clear();
            FranchisePicks.Clear();

            // Load user team players
            if (_currentTradeData.TeamPlayers?.Any() ?? false)
            {
                foreach (var player in _currentTradeData.TeamPlayers)
                {
                    UserPlayers.Add(new TradePlayerItem(player, true));
                }
            }

            // Load user team picks (filter out already used picks if coming from draft)
            if (_currentTradeData.TeamPicks?.Any() ?? false)
            {
                foreach (var pick in _currentTradeData.TeamPicks)
                {
                    // If coming from draft, only show picks after current pick
                    if (CurrentPick == -1 || pick > CurrentPick)
                    {
                        UserPicks.Add(new TradePickItem(pick, true));
                    }
                }
            }

            // Load franchise players
            if (_currentTradeData.FranchisePlayers?.Any() ?? false)
            {
                foreach (var player in _currentTradeData.FranchisePlayers)
                {
                    FranchisePlayers.Add(new TradePlayerItem(player, false));
                }
            }

            // Load franchise picks (filter out already used picks if coming from draft)
            if (_currentTradeData.FranchisePicks?.Any() ?? false)
            {
                foreach (var pick in _currentTradeData.FranchisePicks)
                {
                    // If coming from draft, only show picks after current pick
                    if (CurrentPick == -1 || pick > CurrentPick)
                    {
                        FranchisePicks.Add(new TradePickItem(pick, false));
                    }
                }
            }

            // Update visibility flags
            HasUserPlayers = UserPlayers.Any();
            HasUserPicks = UserPicks.Any();
            HasFranchisePlayers = FranchisePlayers.Any();
            HasFranchisePicks = FranchisePicks.Any();
        }

        private void UpdateTradeCalculations()
        {
            // Calculate user total value
            _userTotalValue = 0;
            _userTotalValue += UserPlayers.Where(p => p.IsSelected).Sum(p => DraftPicks.GetPlayerValue(p.Player));
            _userTotalValue += UserPicks.Where(p => p.IsSelected).Sum(p => DraftPicks.GetPickValue(p.Pick));

            // Calculate franchise total value
            _franchiseTotalValue = 0;
            _franchiseTotalValue += FranchisePlayers.Where(p => p.IsSelected).Sum(p => DraftPicks.GetPlayerValue(p.Player));
            _franchiseTotalValue += FranchisePicks.Where(p => p.IsSelected).Sum(p => DraftPicks.GetPickValue(p.Pick));

            // Update display texts
            UserTotalValueText = $"Your Value: {_userTotalValue}";
            FranchiseTotalValueText = $"Their Value: {_franchiseTotalValue}";

            // Update trade balance
            var totalValue = _userTotalValue + _franchiseTotalValue;
            if (totalValue == 0)
            {
                TradeBalanceText = "Select items to trade";
                UserValueBarWidth = 0;
                FranchiseValueBarWidth = 0;
            }
            else
            {
                var userPercentage = (double)_userTotalValue / totalValue;
                var franchisePercentage = (double)_franchiseTotalValue / totalValue;

                UserValueBarWidth = userPercentage * 300; // Max width of progress bar
                FranchiseValueBarWidth = franchisePercentage * 300;

                if (_userTotalValue > _franchiseTotalValue)
                {
                    TradeBalanceText = "You're giving more";
                }
                else if (_franchiseTotalValue > _userTotalValue)
                {
                    TradeBalanceText = "You're getting more";
                }
                else
                {
                    TradeBalanceText = "Balanced trade";
                }
            }

            // Update selected items summaries
            UpdateSelectedItemsSummary();

            // Update trade availability
            CanTrade = (_userTotalValue > 0 || _franchiseTotalValue > 0) &&
                       (UserPlayers.Any(p => p.IsSelected) || UserPicks.Any(p => p.IsSelected)) &&
                       (FranchisePlayers.Any(p => p.IsSelected) || FranchisePicks.Any(p => p.IsSelected));
        }

        private void UpdateSelectedItemsSummary()
        {
            SelectedUserItems.Clear();
            SelectedFranchiseItems.Clear();

            // Add selected user players
            foreach (var player in UserPlayers.Where(p => p.IsSelected))
            {
                SelectedUserItems.Add(new TradeItemSummary($"🏃 {player.Name} ({player.Position})", DraftPicks.GetPlayerValue(player.Player)));
            }

            // Add selected user picks
            foreach (var pick in UserPicks.Where(p => p.IsSelected))
            {
                SelectedUserItems.Add(new TradeItemSummary($"🏆 Pick #{pick.Pick}", DraftPicks.GetPickValue(pick.Pick)));
            }

            // Add selected franchise players
            foreach (var player in FranchisePlayers.Where(p => p.IsSelected))
            {
                SelectedFranchiseItems.Add(new TradeItemSummary($"🏃 {player.Name} ({player.Position})", DraftPicks.GetPlayerValue(player.Player)));
            }

            // Add selected franchise picks
            foreach (var pick in FranchisePicks.Where(p => p.IsSelected))
            {
                SelectedFranchiseItems.Add(new TradeItemSummary($"🏆 Pick #{pick.Pick}", DraftPicks.GetPickValue(pick.Pick)));
            }
        }

        private void UpdateTabVisibility(string activeTab)
        {
            // Reset all tabs
            IsSummaryTabVisible = false;
            IsUserTabVisible = false;
            IsFranchiseTabVisible = false;

            SummaryTabColor = Color.FromArgb("#f8f9fa");
            SummaryTabTextColor = Color.FromArgb("#6c757d");
            UserTabColor = Color.FromArgb("#f8f9fa");
            UserTabTextColor = Color.FromArgb("#6c757d");
            FranchiseTabColor = Color.FromArgb("#f8f9fa");
            FranchiseTabTextColor = Color.FromArgb("#6c757d");

            // Set active tab
            switch (activeTab)
            {
                case "summary":
                    IsSummaryTabVisible = true;
                    SummaryTabColor = Color.FromArgb("#007bff");
                    SummaryTabTextColor = Colors.White;
                    break;
                case "user":
                    IsUserTabVisible = true;
                    UserTabColor = Color.FromArgb("#007bff");
                    UserTabTextColor = Colors.White;
                    break;
                case "franchise":
                    IsFranchiseTabVisible = true;
                    FranchiseTabColor = Color.FromArgb("#007bff");
                    FranchiseTabTextColor = Colors.White;
                    break;
            }
        }

        private async Task SubmitTrade(bool force)
        {
            try
            {
                UpdateLoadingState(true, "Submitting trade...");

                if (_currentTradeData == null) return;

                // Build trade DTO
                var tradeDto = new TradeDto
                {
                    TeamId = TeamId,
                    FranchiseId = SelectedFranchiseId,
                    Force = force,
                    TeamPlayers = UserPlayers.Where(p => p.IsSelected).Select(p => p.Player).ToList(),
                    TeamPicks = UserPicks.Where(p => p.IsSelected).Select(p => p.Pick).ToList(),
                    FranchisePlayers = FranchisePlayers.Where(p => p.IsSelected).Select(p => p.Player).ToList(),
                    FranchisePicks = FranchisePicks.Where(p => p.IsSelected).Select(p => p.Pick).ToList()
                };

                var success = await _teamService.SaveTradeAsync(tradeDto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Trade completed successfully!", "OK");
                    await ReturnToOrigin(tradeDto);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Trade Failed", "Trade was not accepted. Try adjusting values or use Force Trade.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to submit trade: {ex.Message}", "OK");
            }
            finally
            {
                UpdateLoadingState(false);
            }
        }

        private async Task ReturnToOrigin(TradeDto? completedTrade = null)
        {
            try
            {
                if (CurrentPick == -1)
                    await BaseService.GoToAsync(AppRoutes.Roster, new() { ["TeamId"] = TeamId });
                else
                {
                    // Coming from draft - return with trade data
                    Dictionary<string, object>? parameters = new() { ["teamId"] = TeamId, ["currentPick"] = CurrentPick };

                    // Add trade result data if trade was completed
                    if (completedTrade != null)
                    {
                        parameters["tradedFranchiseId"] = completedTrade.FranchiseId;
                        parameters["userPicksSent"] = completedTrade.TeamPicks ?? new List<int>();
                        parameters["franchisePicksSent"] = completedTrade.FranchisePicks ?? new List<int>();
                    }

                    await BaseService.GoToAsync(AppRoutes.Roster, parameters);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }

        private void LoadAvailableFranchises()
        {
            _availableFranchises = FranchiseInfo.GetAllFranchises().ToList();
        }

        private void UpdateLoadingState(bool loading, string message = "Loading...")
        {
            IsLoading = loading;
            LoadingMessage = message;
        }
    }

    // Supporting classes
    public partial class TradePlayerItem : ObservableObject
    {
        public SelectableDto Player { get; }
        public string Name => Player.Name;
        public string Position => Player.Position;
        public string ValueText => $"Value: {DraftPicks.GetPlayerValue(Player)}";
        public bool IsUserPlayer { get; }

        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private Color selectionColor = Colors.White;
        [ObservableProperty] private string selectionIcon = "○";
        [ObservableProperty] private Color selectionIconColor = Color.FromArgb("#6c757d");

        public TradePlayerItem(SelectableDto player, bool isUserPlayer)
        {
            Player = player;
            IsUserPlayer = isUserPlayer;
        }

        partial void OnIsSelectedChanged(bool value)
        {
            if (value)
            {
                SelectionColor = Color.FromArgb("#e3f2fd");
                SelectionIcon = "●";
                SelectionIconColor = IsUserPlayer ? Color.FromArgb("#007bff") : Color.FromArgb("#dc3545");
            }
            else
            {
                SelectionColor = Colors.White;
                SelectionIcon = "○";
                SelectionIconColor = Color.FromArgb("#6c757d");
            }
        }
    }

    public partial class TradePickItem : ObservableObject
    {
        public int Pick { get; }
        public string DisplayText => $"Pick #{Pick}";
        public string ValueText => $"Value: {DraftPicks.GetPickValue(Pick)}";
        public bool IsUserPick { get; }

        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private Color selectionColor = Colors.White;
        [ObservableProperty] private string selectionIcon = "○";
        [ObservableProperty] private Color selectionIconColor = Color.FromArgb("#6c757d");

        public TradePickItem(int pick, bool isUserPick)
        {
            Pick = pick;
            IsUserPick = isUserPick;
        }

        partial void OnIsSelectedChanged(bool value)
        {
            if (value)
            {
                SelectionColor = Color.FromArgb("#e3f2fd");
                SelectionIcon = "●";
                SelectionIconColor = IsUserPick ? Color.FromArgb("#007bff") : Color.FromArgb("#dc3545");
            }
            else
            {
                SelectionColor = Colors.White;
                SelectionIcon = "○";
                SelectionIconColor = Color.FromArgb("#6c757d");
            }
        }
    }

    public class TradeItemSummary
    {
        public string DisplayText { get; }
        public int Value { get; }

        public TradeItemSummary(string displayText, int value)
        {
            DisplayText = displayText;
            Value = value;
        }
    }
}