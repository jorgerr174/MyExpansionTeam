using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class TradesViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public TradesViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private IList<TradeDisplayInfo> trades = new List<TradeDisplayInfo>();
        [ObservableProperty] private IList<TradeDisplayInfo> filteredTrades = new List<TradeDisplayInfo>();
        [ObservableProperty] private string selectedYearFilter = "All";
        [ObservableProperty] private bool hasNoTrades = false;

        public List<string> YearFilters { get; private set; } = new() { "All" };

        [RelayCommand]
        public async Task LoadTrades(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                var teamTrades = await _teamService.GetTeamTradesAsync(id);
                if (teamTrades != null)
                {
                    var tradeDisplayList = teamTrades.Select(trade => new TradeDisplayInfo
                    {
                        Id = trade.Id,
                        Date = trade.Date,
                        FranchiseId = trade.FranchiseId,
                        FranchiseName = GetFranchiseName(trade.FranchiseId),
                        TeamPlayers = trade.TeamPlayers,
                        TeamPicks = trade.TeamPicks,
                        FranchisePlayers = trade.FranchisePlayers,
                        FranchisePicks = trade.FranchisePicks,
                        TeamCurrentCap = trade.TeamCurrentCap,
                        IsForced = trade.Force
                    }).OrderByDescending(t => t.Date).ToList();

                    Trades = tradeDisplayList;

                    // Generate year filters
                    var years = tradeDisplayList.Select(t => t.Date.Year.ToString()).Distinct().OrderByDescending(y => y).ToList();
                    YearFilters = new List<string> { "All" }.Concat(years).ToList();
                    OnPropertyChanged(nameof(YearFilters));

                    ApplyYearFilter();
                    HasNoTrades = !Trades.Any();
                }
                else
                {
                    Trades = new List<TradeDisplayInfo>();
                    HasNoTrades = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load trades: {ex.Message}";
                HasNoTrades = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void FilterByYear(string year)
        {
            SelectedYearFilter = year;
            ApplyYearFilter();
        }

        [RelayCommand]
        public async Task ViewTradeDetails(TradeDisplayInfo trade)
        {
            // Show detailed trade information
            var tradeDetails = CreateTradeDetailsText(trade);
            await Shell.Current.DisplayAlert("Trade Details", tradeDetails, "OK");
        }

        [RelayCommand]
        public async Task GoToNewTrade()
        {
            await Shell.Current.GoToAsync($"Trade?teamId={TeamId}");
        }

        private void ApplyYearFilter()
        {
            if (SelectedYearFilter == "All")
            {
                FilteredTrades = Trades;
            }
            else
            {
                if (int.TryParse(SelectedYearFilter, out int year))
                {
                    FilteredTrades = Trades.Where(t => t.Date.Year == year).ToList();
                }
                else
                {
                    FilteredTrades = Trades;
                }
            }
        }

        private string GetFranchiseName(int franchiseId)
        {
            var franchise = FranchiseInfo.GetAllFranchises().FirstOrDefault(f => f.Id == franchiseId);
            return franchise?.Name ?? $"Franchise {franchiseId}";
        }

        private string CreateTradeDetailsText(TradeDisplayInfo trade)
        {
            var details = $"Trade with {trade.FranchiseName}\n";
            details += $"Date: {trade.Date:yyyy-MM-dd}\n";
            details += $"Salary Cap Impact: ${trade.TeamCurrentCap:F1}M\n";

            if (trade.IsForced)
                details += "⚠️ Forced Trade\n";

            details += "\n--- Your Team Gave ---\n";

            if (trade.TeamPlayers.Any())
            {
                details += "Players:\n";
                foreach (var player in trade.TeamPlayers)
                {
                    details += $"• {player.Name} ({player.Position}) - {player.APY}\n";
                }
            }

            if (trade.TeamPicks.Any())
            {
                details += "Draft Picks:\n";
                foreach (var pick in trade.TeamPicks)
                {
                    details += $"• Pick #{pick}\n";
                }
            }

            details += "\n--- Your Team Received ---\n";

            if (trade.FranchisePlayers.Any())
            {
                details += "Players:\n";
                foreach (var player in trade.FranchisePlayers)
                {
                    details += $"• {player.Name} ({player.Position}) - {player.APY}\n";
                }
            }

            if (trade.FranchisePicks.Any())
            {
                details += "Draft Picks:\n";
                foreach (var pick in trade.FranchisePicks)
                {
                    details += $"• Pick #{pick}\n";
                }
            }

            return details;
        }
    }

    public class TradeDisplayInfo
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int FranchiseId { get; set; }
        public string FranchiseName { get; set; } = string.Empty;
        public IList<SelectableDto> TeamPlayers { get; set; } = new List<SelectableDto>();
        public IList<int> TeamPicks { get; set; } = new List<int>();
        public IList<SelectableDto> FranchisePlayers { get; set; } = new List<SelectableDto>();
        public IList<int> FranchisePicks { get; set; } = new List<int>();
        public decimal TeamCurrentCap { get; set; }
        public bool IsForced { get; set; }

        public string DateString => Date.ToString("yyyy-MM-dd");
        public string TradeTitle => $"Trade with {FranchiseName}";
        public string TradeSubtitle => $"{Date:MMM dd, yyyy} • Cap: ${TeamCurrentCap:F1}M";

        public int TotalItemsGiven => TeamPlayers.Count + TeamPicks.Count;
        public int TotalItemsReceived => FranchisePlayers.Count + FranchisePicks.Count;

        public string TradeSummary => $"Gave {TotalItemsGiven} items • Received {TotalItemsReceived} items";
    }
}