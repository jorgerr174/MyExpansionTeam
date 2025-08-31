using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class TradesViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;

        [ObservableProperty] private int teamId;
        [ObservableProperty] private string teamName = string.Empty;
        [ObservableProperty] private IList<TradeDisplayInfo> trades = [];
        [ObservableProperty] private IList<TradeDisplayInfo> filteredTrades = [];
        [ObservableProperty] private string selectedYearFilter = "All";
        [ObservableProperty] private bool hasNoTrades = false;

        public List<string> YearFilters { get; private set; } = ["All"];


        [RelayCommand] public async Task GoToTrade() => await BaseService.GoToAsync(AppRoutes.Trade, new() { ["TeamId"] = TeamId });


        [RelayCommand]
        public async Task LoadTrades(int id)
        {
            TeamId = id;
            IsLoading = true;

            try
            {
                if (await _teamService.GetTeamTradesAsync(id) is IList<TradeDto> teamTrades)
                {
                    IList<TradeDisplayInfo> tradeDisplayList = [.. teamTrades.Select(trade => new TradeDisplayInfo
                    {
                        Id = trade.Id,
                        Date = trade.Date,
                        FranchiseId = trade.FranchiseId,
                        FranchiseName = TradesViewModel.GetFranchiseName(trade.FranchiseId),
                        TeamPlayers = trade.TeamPlayers,
                        TeamPicks = trade.TeamPicks,
                        FranchisePlayers = trade.FranchisePlayers,
                        FranchisePicks = trade.FranchisePicks,
                        TeamCurrentCap = trade.TeamCurrentCap,
                        IsForced = trade.Force
                    }).OrderByDescending(t => t.Date)];

                    Trades = tradeDisplayList;

                    // Generate year filters
                    var years = tradeDisplayList.Select(t => t.Date.Year.ToString()).Distinct().OrderByDescending(y => y).ToList();
                    YearFilters = ["All", .. years];
                    OnPropertyChanged(nameof(YearFilters));

                    ApplyYearFilter();
                    HasNoTrades = !Trades.Any();
                }
                else
                {
                    Trades = [];
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
        public static async Task ViewTradeDetails(TradeDisplayInfo trade)
            => await Shell.Current.DisplayAlert("Detalles del Trueque", TradesViewModel.CreateTradeDetailsText(trade), "OK");

        private void ApplyYearFilter()
            => FilteredTrades = SelectedYearFilter == "All" ? Trades
                : int.TryParse(SelectedYearFilter, out int year) ? [.. Trades.Where(t => t.Date.Year == year)] : Trades;

        private static string GetFranchiseName(int franchiseId)
        {
            var franchise = FranchiseInfo.GetAllFranchises()[franchiseId - 1];
            return franchise?.Name ?? $"Franchise {franchiseId}";
        }

        private static string CreateTradeDetailsText(TradeDisplayInfo trade)
        {
            string details = $"Trade with {trade.FranchiseName}\n";
            details += $"Date: {trade.Date:yyyy-MM-dd}\n";
            details += $"Salary Cap Impact: ${trade.TeamCurrentCap:F1}M\n";

            if (trade.IsForced)
                details += "⚠️ Forced Trade\n";

            details += "\n--- Your Team Gave ---\n";

            if (trade.TeamPlayers.Any())
            {
                details += "Players:\n";
                foreach (SelectableDto player in trade.TeamPlayers)
                    details += $"• {player.Name} ({player.Position}) - {player.APY}\n";
            }

            if (trade.TeamPicks.Any())
            {
                details += "Draft Picks:\n";
                foreach (int pick in trade.TeamPicks)
                    details += $"• Pick #{pick}\n";
            }

            details += "\n--- Your Team Received ---\n";

            if (trade.FranchisePlayers.Any())
            {
                details += "Players:\n";
                foreach (SelectableDto player in trade.FranchisePlayers)
                    details += $"• {player.Name} ({player.Position}) - {player.APY}\n";
            }

            if (trade.FranchisePicks.Any())
            {
                details += "Draft Picks:\n";
                foreach (int pick in trade.FranchisePicks)
                    details += $"• Pick #{pick}\n";
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
        public IList<SelectableDto> TeamPlayers { get; set; } = [];
        public IList<int> TeamPicks { get; set; } = [];
        public IList<SelectableDto> FranchisePlayers { get; set; } = [];
        public IList<int> FranchisePicks { get; set; } = [];
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