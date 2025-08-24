//using System.Collections.ObjectModel;
//using System.Diagnostics.CodeAnalysis;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using METCore.DTOs.Player;
//using METCore.DTOs.Team;
//using MobileApp.Models.Shared;
//using MobileApp.Services;

//namespace MobileApp.Models.Team
//{
//    public class StatusType(Color BackgroundColor, Color BorderColor, string StatusText, Color StatusTextColor, Color StatusBackgroundColor, string Icon, Color IconColor)
//    {
//        public Color BackgroundColor { get; private set; } = BackgroundColor;
//        public Color BorderColor { get; private set; } = BorderColor;

//        public string StatusText { get; private set; } = StatusText;
//        public Color StatusTextColor { get; private set; } = StatusTextColor;
//        public Color StatusBackgroundColor { get; private set; } = StatusBackgroundColor;

//        public string Icon { get; private set; } = Icon;
//        public Color IconColor { get; private set; } = IconColor;
//    }

//    public static class StatusRegistry
//    {
//        public static readonly StatusType TradedAway = new(Color.FromArgb("#f8d7da"), Color.FromArgb("#f5c6cb"), "TRADED AWAY", Color.FromArgb("#721c24"), Color.FromArgb("#f8d7da"), "🔒", Color.FromArgb("#dc3545"));
//        public static readonly StatusType TradedFor= new(Color.FromArgb("#d4edda"), Color.FromArgb("#28a745"), "TRADED FOR", Color.FromArgb("#155724"), Color.FromArgb("#d4edda"), "✓", Color.FromArgb("#28a745"));
//        public static readonly StatusType Protected = new(Color.FromArgb("#fff3cd"), Color.FromArgb("#ffeaa7"), "Protected player - cannot be added", Color.FromArgb("#856404"), Color.FromArgb("#fff3cd"), "🔒", Color.FromArgb("#856404"));
//        public static readonly StatusType Selected = new(Color.FromArgb("#d4edda"), Color.FromArgb("#28a745"), "SELECTED", Color.FromArgb("#721c24"), Color.FromArgb("#f8d7da"), "✓", Color.FromArgb("#28a745"));
//        public static readonly StatusType Available = new(Colors.White, Color.FromArgb("#dee2e6"), "AVAILABLE", Colors.Black, Colors.Black, string.Empty, Colors.White);
//    }

//    public partial class PlayerModel : ObservableObject
//    {
//        [ObservableProperty] private SelectableDto player;

//        [NotNull] [ObservableProperty] private StatusType playerStatus;

//        public bool IsTradedAway => ReferenceEquals(playerStatus, StatusRegistry.TradedAway);
//        public bool IsTradedFor => ReferenceEquals(playerStatus, StatusRegistry.TradedFor);
//        public bool IsProtected => ReferenceEquals(playerStatus, StatusRegistry.Protected);
//        public bool IsSelected => ReferenceEquals(playerStatus, StatusRegistry.Selected);
//        public bool isAvailable => ReferenceEquals(playerStatus, StatusRegistry.Available);
//        public bool Clickable => IsSelected || isAvailable;
//        public int Id => Player.Id;


//        public PlayerModel(SelectableDto player, bool traded, bool prot, bool selected)
//        {
//            Player = player;
//            PlayerStatus =
//                traded ? StatusRegistry.TradedAway
//                : prot && selected ? StatusRegistry.TradedFor 
//                    : prot ? StatusRegistry.Protected 
//                        : selected ? StatusRegistry.Selected
//                            : StatusRegistry.Available;
//        }

//        public void TogglePlayer() => this.PlayerStatus = isAvailable ? StatusRegistry.Selected : StatusRegistry.Available;
//    }

//    public partial class PositionGroupModel : ObservableObject
//    {
//        private string position => Players?[0]?.Player?.Position ?? string.Empty;
//        public ObservableCollection<PlayerModel> Players { get; } = [];

//        public PositionGroupModel(IList<PlayerModel> players) 
//        {
//            foreach(PlayerModel player in players) Players.Add(player);
//        }
//    }

//    public partial class FranchiseModel : ObservableObject
//    {
//        [ObservableProperty] private FranchiseInfo franchiseInfo;
//        [ObservableProperty] private int selectedCount;
//        [ObservableProperty] private Color backgroundColor;

//        public ObservableCollection<PositionGroupModel> PositionGroups { get; } = [];
//        private int PlayerCount => PositionGroups.SelectMany(pg => pg.Players).Count();

//        public FranchiseModel(FranchiseInfo info)
//        {
//            FranchiseInfo = info;
//            BackgroundColor = Color.FromArgb("#f8f9fa");
//            SelectedCount = 0;
//        }
//        public FranchiseModel(FranchiseInfo info, int selectedCount) : this(info)
//        {
//            SelectedCount = selectedCount;
//        }
//    }
//}