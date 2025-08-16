using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class TradeViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public TradeViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private string tradeContext = "roster"; // "roster" or "draft"
        [ObservableProperty] private int currentPick = -1;

        // Franchise selection
        [ObservableProperty] private List<FranchiseInfo> franchises = FranchiseInfo.GetAllFranchises();
        [ObservableProperty] private FranchiseInfo? selectedFranchise;
        [ObservableProperty] private bool showFranchiseSelection = true;
        [ObservableProperty] private bool showTradeBuilder = false;

        // Available items for trade
        [ObservableProperty] private IList<SelectableDto> availableTeamPlayers = new List<SelectableDto>();
        [ObservableProperty] private IList<SelectableDto> availableFranchisePlayers = new List<SelectableDto>();
        [ObservableProperty] private IList<string> availableTeamPicks = new List<string>();
        [ObservableProperty] private IList<string> availableFranchisePicks = new List<string>();

        // Selected items for trade
        [ObservableProperty] private IList<SelectableDto> selectedTeamPlayers = new List<SelectableDto>();
        [ObservableProperty] private IList<SelectableDto> selectedFranchisePlayers = new List<SelectableDto>();
        [ObservableProperty] private IList<string> selectedTeamPicks = new List<string>();
        [ObservableProperty] private IList<string> selectedFranchisePicks = new List<string>();

        // Trade values and validation
        [ObservableProperty] private decimal teamTradeValue = 0;
        [ObservableProperty] private decimal franchiseTradeValue = 0;
        [ObservableProperty] private decimal teamCurrentCap = 0;
        [ObservableProperty] private bool isValidTrade = false;

        public bool HasSelectedItems =>
            SelectedTeamPlayers.Any() || SelectedTeamPicks.Any() ||
            SelectedFranchisePlayers.Any() || SelectedFranchisePicks.Any();

        public string TradeValueComparison =>
            $"Your Value: {TeamTradeValue:F1} | Their Value: {FranchiseTradeValue:F1}";

        [RelayCommand]
        public async Task LoadTradeData()
        {
            IsLoading = true;

            try
            {
                // For now, just enable franchise selection
                // Team name will be loaded when franchise is selected
                ShowFranchiseSelection = true;
                ShowTradeBuilder = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to initialize trade: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task InitializeTradeAsync(int id, string context = "roster", int pick = -1)
        {
            TeamId = id;
            TradeContext = context;
            CurrentPick = pick;
            IsLoading = true;

            try
            {
                // For now, just enable franchise selection
                // Team name will be loaded when franchise is selected
                ShowFranchiseSelection = true;
                ShowTradeBuilder = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to initialize trade: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SelectFranchise(FranchiseInfo franchise)
        {
            SelectedFranchise = franchise;
            IsLoading = true;

            try
            {
                var tradeData = await _teamService.GetTradeDataAsync(TeamId, franchise.Id);
                if (tradeData != null)
                {
                    TeamName = $"{tradeData.TeamPlayers.FirstOrDefault()?.Name ?? "Your Team"}"; // Will need proper team name

                    // Load available items
                    AvailableTeamPlayers = tradeData.TeamPlayers;
                    AvailableFranchisePlayers = tradeData.FranchisePlayers;
                    AvailableTeamPicks = tradeData.TeamPicks.Select(p => FormatPickAsString(p)).ToList();
                    AvailableFranchisePicks = tradeData.FranchisePicks.Select(p => FormatPickAsString(p)).ToList();
                    TeamCurrentCap = tradeData.TeamCurrentCap;

                    // Clear selections
                    ClearSelections();

                    ShowFranchiseSelection = false;
                    ShowTradeBuilder = true;
                }
                else
                {
                    ErrorMessage = "Failed to load trade data";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load trade data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ToggleTeamPlayer(SelectableDto player)
        {
            if (SelectedTeamPlayers.Contains(player))
            {
                SelectedTeamPlayers = SelectedTeamPlayers.Where(p => p.Id != player.Id).ToList();
            }
            else
            {
                SelectedTeamPlayers = SelectedTeamPlayers.Append(player).ToList();
            }
            CalculateTradeValues();
        }

        [RelayCommand]
        public void ToggleFranchisePlayer(SelectableDto player)
        {
            if (SelectedFranchisePlayers.Contains(player))
            {
                SelectedFranchisePlayers = SelectedFranchisePlayers.Where(p => p.Id != player.Id).ToList();
            }
            else
            {
                SelectedFranchisePlayers = SelectedFranchisePlayers.Append(player).ToList();
            }
            CalculateTradeValues();
        }

        [RelayCommand]
        public void ToggleTeamPick(string pick)
        {
            if (SelectedTeamPicks.Contains(pick))
            {
                SelectedTeamPicks = SelectedTeamPicks.Where(p => p != pick).ToList();
            }
            else
            {
                SelectedTeamPicks = SelectedTeamPicks.Append(pick).ToList();
            }
            CalculateTradeValues();
        }

        [RelayCommand]
        public void ToggleFranchisePick(string pick)
        {
            if (SelectedFranchisePicks.Contains(pick))
            {
                SelectedFranchisePicks = SelectedFranchisePicks.Where(p => p != pick).ToList();
            }
            else
            {
                SelectedFranchisePicks = SelectedFranchisePicks.Append(pick).ToList();
            }
            CalculateTradeValues();
        }

        [RelayCommand]
        public void BackToFranchiseSelection()
        {
            ShowTradeBuilder = false;
            ShowFranchiseSelection = true;
            SelectedFranchise = null;
            ClearSelections();
        }

        [RelayCommand]
        public async Task RequestTrade()
        {
            await SubmitTrade(false);
        }

        [RelayCommand]
        public async Task ForceTrade()
        {
            await SubmitTrade(true);
        }

        [RelayCommand]
        public async Task CancelTrade()
        {
            await GoBackToCaller();
        }

        private async Task SubmitTrade(bool force)
        {
            if (!HasSelectedItems)
            {
                ErrorMessage = "Please select items to trade";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var tradeDto = new TradeDto(TeamId, SelectedFranchise!.Id)
                {
                    Force = force,
                    TeamCurrentCap = TeamCurrentCap,
                    TeamPlayers = SelectedTeamPlayers,
                    FranchisePlayers = SelectedFranchisePlayers,
                    TeamPicks = SelectedTeamPicks.Select(ParsePickFromString).ToList(),
                    FranchisePicks = SelectedFranchisePicks.Select(ParsePickFromString).ToList()
                };

                bool success = await _teamService.SaveTradeAsync(tradeDto);

                if (success)
                {
                    await HandleTradeSuccess(tradeDto);
                }
                else
                {
                    ErrorMessage = force ? "Failed to force trade" : "Trade was rejected";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Trade failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task HandleTradeSuccess(TradeDto tradeDto)
        {
            if (TradeContext == "draft")
            {
                // Return to draft with trade result
                var tradeResult = new
                {
                    Success = true,
                    TeamTraded = tradeDto.TeamPicks,
                    FranchiseTraded = tradeDto.FranchisePicks,
                    FranchiseId = tradeDto.FranchiseId
                };

                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    ["tradeResult"] = tradeResult
                });
            }
            else
            {
                // Return to roster/team details (will reload)
                await Shell.Current.GoToAsync("MyTeams");
            }
        }

        private async Task GoBackToCaller()
        {
            if (TradeContext == "draft")
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.GoToAsync("MyTeams");
            }
        }

        private void ClearSelections()
        {
            SelectedTeamPlayers = new List<SelectableDto>();
            SelectedFranchisePlayers = new List<SelectableDto>();
            SelectedTeamPicks = new List<string>();
            SelectedFranchisePicks = new List<string>();
            TeamTradeValue = 0;
            FranchiseTradeValue = 0;
        }

        private void CalculateTradeValues()
        {
            // Basic trade value calculation
            // TODO: Implement proper value calculation using DraftPicks.GetPickValue()
            // For now, use simple approximation

            TeamTradeValue = 0;
            FranchiseTradeValue = 0;

            // Calculate pick values (simplified)
            foreach (var pick in SelectedTeamPicks)
            {
                TeamTradeValue += GetSimplePickValue(pick);
            }

            foreach (var pick in SelectedFranchisePicks)
            {
                FranchiseTradeValue += GetSimplePickValue(pick);
            }

            // Player values (use APY as rough estimate)
            foreach (var player in SelectedTeamPlayers)
            {
                if (decimal.TryParse(player.PureAPY, out decimal value))
                    TeamTradeValue += value * 10; // Rough conversion
            }

            foreach (var player in SelectedFranchisePlayers)
            {
                if (decimal.TryParse(player.PureAPY, out decimal value))
                    FranchiseTradeValue += value * 10; // Rough conversion
            }

            // Update UI properties
            OnPropertyChanged(nameof(TradeValueComparison));
            OnPropertyChanged(nameof(HasSelectedItems));
        }

        private decimal GetSimplePickValue(string pick)
        {
            // Simple pick value calculation - replace with proper DraftPicks.GetPickValue()
            if (pick.StartsWith("r1")) return 1000;
            if (pick.StartsWith("r2")) return 500;
            if (pick.StartsWith("r3")) return 250;
            if (pick.StartsWith("r4")) return 125;
            if (pick.StartsWith("r5")) return 60;
            if (pick.StartsWith("r6")) return 30;
            if (pick.StartsWith("r7")) return 15;
            return 0;
        }

        private string FormatPickAsString(int pick)
        {
            // Convert pick number to "r1p1" format
            int round = ((pick - 1) / 32) + 1;
            int pickInRound = ((pick - 1) % 32) + 1;
            return $"r{round}p{pickInRound}";
        }

        private int ParsePickFromString(string pick)
        {
            // Convert "r1p1" format to pick number
            var parts = pick.Replace("r", "").Split('p');
            if (parts.Length == 2 && int.TryParse(parts[0], out int round) && int.TryParse(parts[1], out int pickInRound))
            {
                return ((round - 1) * 32) + pickInRound;
            }
            return 0;
        }
    }
}