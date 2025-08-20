using METCore.DTOs.Player;
using METCore.DTOs.Team;

namespace MobileApp.Models.Team
{
    public class DraftState
    {
        public int TeamId { get; set; }
        public DraftDto OriginalDraftData { get; set; }
        public string DraftMethod { get; set; } = "full";
        public int ManualRounds { get; set; } = 0;
        public HashSet<int> SelectedFranchises { get; set; } = [];
        public int CurrentPickIndex { get; set; } = 0;
        public List<DraftPickInfo> DraftOrder { get; set; } = [];
        public Dictionary<int, int> AllSelections { get; set; } = [];
        public List<ProspectDto> AvailableProspects { get; set; } = [];
        public bool IsPaused { get; set; } = true;
        public bool IsComplete { get; set; } = false;
        public bool HasUnsavedProgress => AllSelections.Count > 0;

        public DraftPickInfo? CurrentPick =>
            CurrentPickIndex < DraftOrder.Count ? DraftOrder[CurrentPickIndex] : null;

        public bool IsCurrentPickUserControlled =>
            CurrentPick != null && IsPickUserControlled(CurrentPick);

        public void RecordPick(int pickNumber, int playerId)
        {
            AllSelections[pickNumber] = playerId;
            var prospect = AvailableProspects.FirstOrDefault(p => p.Id == playerId);
            if (prospect != null)
            {
                AvailableProspects.Remove(prospect);
            }
        }

        public void AdvanceToNextPick()
        {
            CurrentPickIndex++;
            IsComplete = CurrentPickIndex >= DraftOrder.Count;
        }

        public bool IsPickUserControlled(DraftPickInfo pick)
            => DraftMethod != "full"
                && pick.Round <= ManualRounds && ManualRounds != 0
                && (pick.IsUserTeam
                    || DraftMethod == "multiple" && SelectedFranchises.Contains(pick.TeamId));

        public void BuildDraftOrder()
        {
            DraftOrder.Clear();
            IList<DraftPickInfo> allPicks = [];

            for (int entityIndex = 0; entityIndex < OriginalDraftData.Picks.Count; entityIndex++)
                foreach (var pickNum in OriginalDraftData.Picks[entityIndex])
                    allPicks.Add(ConvertRppToPickInfo(pickNum, entityIndex));

            DraftOrder = [.. allPicks.OrderBy(p => p.Overall)];
        }

        private DraftPickInfo ConvertRppToPickInfo(int rppFormat, int entityIndex)
        {
            int round = rppFormat / 100;
            int pickInRound = rppFormat % 100;
            int overall = ((round - 1) * 32) + pickInRound;

            return new DraftPickInfo
            {
                Overall = overall,
                Round = round,
                PickInRound = pickInRound,
                RppFormat = rppFormat,
                TeamId = entityIndex,
                IsUserTeam = entityIndex == 0,
                TeamName = GetTeamName(entityIndex),
                TeamAbbr = GetTeamAbbreviation(entityIndex)
            };
        }

        private string GetTeamName(int entityIndex)
            => entityIndex == 0
            ? $"{OriginalDraftData.Location} {OriginalDraftData.Mascot}"
            : FranchiseHelper.GetFranchiseName(entityIndex);

        private string GetTeamAbbreviation(int entityIndex)
            => entityIndex == 0
            ? OriginalDraftData.Abb
            : FranchiseHelper.GetFranchiseAbbreviation(entityIndex);

        public DraftDto CreateDraftDto()
        {
            IDictionary<int, int> updatedSelections = OriginalDraftData.Selections;

            foreach (var selection in AllSelections)
                updatedSelections[selection.Key] = selection.Value;

            return new DraftDto(
                OriginalDraftData.Id,
                OriginalDraftData.Location,
                OriginalDraftData.Abb,
                OriginalDraftData.Mascot,
                OriginalDraftData.UserUsername,
                OriginalDraftData.Date,
                IsComplete,
                ManualRounds,
                updatedSelections
            )
            {
                Picks = OriginalDraftData.Picks,
                Prospects = OriginalDraftData.Prospects
            };
        }
    }

    public class DraftPickInfo
    {
        public int Overall { get; set; }
        public int Round { get; set; }
        public int PickInRound { get; set; }
        public int RppFormat { get; set; }
        public int TeamId { get; set; }
        public bool IsUserTeam { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamAbbr { get; set; } = string.Empty;
        public ProspectDto? SelectedPlayer { get; set; }
    }

    public static class FranchiseHelper
    {
        private static readonly Dictionary<int, (string Name, string Abbreviation)> Franchises = new()
        {
            { 1, ("Arizona Cardinals", "ARI") },
            { 2, ("Atlanta Falcons", "ATL") },
            { 3, ("Baltimore Ravens", "BAL") },
            { 4, ("Buffalo Bills", "BUF") },
            { 5, ("Carolina Panthers", "CAR") },
            { 6, ("Chicago Bears", "CHI") },
            { 7, ("Cincinnati Bengals", "CIN") },
            { 8, ("Cleveland Browns", "CLE") },
            { 9, ("Dallas Cowboys", "DAL") },
            { 10, ("Denver Broncos", "DEN") },
            { 11, ("Detroit Lions", "DET") },
            { 12, ("Green Bay Packers", "GB") },
            { 13, ("Houston Texans", "HOU") },
            { 14, ("Indianapolis Colts", "IND") },
            { 15, ("Jacksonville Jaguars", "JAX") },
            { 16, ("Kansas City Chiefs", "KC") },
            { 17, ("Las Vegas Raiders", "LV") },
            { 18, ("Los Angeles Chargers", "LAC") },
            { 19, ("Los Angeles Rams", "LAR") },
            { 20, ("Miami Dolphins", "MIA") },
            { 21, ("Minnesota Vikings", "MIN") },
            { 22, ("New England Patriots", "NE") },
            { 23, ("New Orleans Saints", "NO") },
            { 24, ("New York Giants", "NYG") },
            { 25, ("New York Jets", "NYJ") },
            { 26, ("Philadelphia Eagles", "PHI") },
            { 27, ("Pittsburgh Steelers", "PIT") },
            { 28, ("San Francisco 49ers", "SF") },
            { 29, ("Seattle Seahawks", "SEA") },
            { 30, ("Tampa Bay Buccaneers", "TB") },
            { 31, ("Tennessee Titans", "TEN") },
            { 32, ("Washington Commanders", "WAS") }
        };

        public static string GetFranchiseName(int franchiseId)
        {
            return Franchises.TryGetValue(franchiseId, out var franchise) ? franchise.Name : $"Team {franchiseId}";
        }

        public static string GetFranchiseAbbreviation(int franchiseId)
        {
            return Franchises.TryGetValue(franchiseId, out var franchise) ? franchise.Abbreviation : $"T{franchiseId}";
        }

        public static Dictionary<int, (string Name, string Abbreviation)> GetAllFranchises()
        {
            return new Dictionary<int, (string Name, string Abbreviation)>(Franchises);
        }
    }

    public class DraftResultInfo
    {
        public int Round { get; set; }
        public int PickNumber { get; set; }
        public int OverallPick { get; set; }
        public ProspectDto SelectedProspect { get; set; }
        public bool IsUserPick { get; set; }

        public string ProspectName => SelectedProspect?.Name ?? "";
        public string ProspectPosition => SelectedProspect?.Position ?? "";
        public string PickDisplayText => $"Round {Round}, Pick #{PickNumber}";
        public string OverallPickText => $"#{OverallPick} Overall";
        public string StatusText => $"Selected: {ProspectName}";
        public Color StatusColor => IsUserPick ? Colors.Green : Colors.LightGray;
    }
}