using METCore.DTOs.Player;

namespace MobileApp.Models.Team
{
    // Main draft pick info class - used during draft simulation
    public class DraftPickInfo
    {
        public int Overall { get; set; }
        public int Round { get; set; }
        public int PickInRound { get; set; }
        public int TeamId { get; set; }
        public bool IsUserTeam { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int RppFormat { get; set; }

        // Optional - for when pick is made
        public bool IsUsed { get; set; }
        public ProspectDto? SelectedProspect { get; set; }

        // Display properties
        public string PickDisplayText => $"Round {Round}, Pick #{PickInRound}";
        public string OverallPickText => $"#{Overall} Overall";
        public string StatusText => IsUsed && SelectedProspect != null ?
            $"Selected: {SelectedProspect.Name}" :
            IsUserTeam ? "Your Pick" : TeamName;
        public Color StatusColor => IsUsed ? Colors.Green :
                                   IsUserTeam ? Colors.Blue : Colors.Gray;
    }

    // Simpler class for draft results display (when showing completed draft)
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

    // Draft state management class
    public class DraftState
    {
        public int ManualRounds { get; set; } = 3;
        public bool EnableTrading { get; set; } = true;
        public bool IsPaused { get; set; } = false;
        public bool IsComplete { get; set; } = false;
        public int CurrentPickIndex { get; set; } = 0;

        public List<DraftPickInfo> DraftOrder { get; set; } = new();
        public Dictionary<int, int> Selections { get; set; } = new(); // Pick number -> Player ID

        public DraftPickInfo? CurrentPick =>
            CurrentPickIndex < DraftOrder.Count ? DraftOrder[CurrentPickIndex] : null;

        public bool HasUnsavedProgress => Selections.Any();

        public void RecordPick(int pickNumber, int playerId)
        {
            Selections[pickNumber] = playerId;

            // Update the pick info
            var pick = DraftOrder.FirstOrDefault(p => p.Overall == pickNumber);
            if (pick != null)
            {
                pick.IsUsed = true;
            }
        }

        public void AdvanceToNextPick()
        {
            CurrentPickIndex++;
            if (CurrentPickIndex >= DraftOrder.Count)
            {
                IsComplete = true;
            }
        }
    }
}