using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using METCore.DTOs.Player;
using MobileApp.Models.Shared;

namespace MobileApp.Models.Team
{
    public class StatusType(Color BackgroundColor, Color BorderColor, string StatusText, Color StatusTextColor, Color StatusBackgroundColor, string Icon, Color IconColor)
    {
        public Color BackgroundColor { get; private set; } = BackgroundColor;
        public Color BorderColor { get; private set; } = BorderColor;
        public string StatusText { get; private set; } = StatusText;
        public Color StatusTextColor { get; private set; } = StatusTextColor;
        public Color StatusBackgroundColor { get; private set; } = StatusBackgroundColor;
        public string Icon { get; private set; } = Icon;
        public Color IconColor { get; private set; } = IconColor;
    }

    public static class StatusRegistry
    {
        public static readonly StatusType TradedAway = new(Color.FromArgb("#f8d7da"), Color.FromArgb("#f5c6cb"), "Enviado en trueque", Color.FromArgb("#721c24"), Color.FromArgb("#f8d7da"), "🔒", Color.FromArgb("#dc3545"));
        public static readonly StatusType TradedFor = new(Color.FromArgb("#d4edda"), Color.FromArgb("#28a745"), "Recibido en trueque", Color.FromArgb("#155724"), Color.FromArgb("#d4edda"), "✓", Color.FromArgb("#28a745"));
        public static readonly StatusType Protected = new(Color.FromArgb("#fff3cd"), Color.FromArgb("#ffeaa7"), "Protegido - no se puede añadir", Color.FromArgb("#856404"), Color.FromArgb("#fff3cd"), "🔒", Color.FromArgb("#856404"));
        public static readonly StatusType Selected = new(Color.FromArgb("#d4edda"), Color.FromArgb("#28a745"), "SELECCIONADO", Color.FromArgb("#155724"), Color.FromArgb("#d4edda"), "✓", Color.FromArgb("#28a745"));
        public static readonly StatusType Available = new(Colors.White, Color.FromArgb("#dee2e6"), "DISPONIBLE", Colors.Black, Colors.Transparent, string.Empty, Colors.Transparent);
    }

    public partial class PlayerModel : ObservableObject
    {
        [ObservableProperty] private SelectableDto player;
        [ObservableProperty] private StatusType playerStatus;

        public bool StatusTradedAway => ReferenceEquals(PlayerStatus, StatusRegistry.TradedAway);
        public bool StatusTradedFor => ReferenceEquals(PlayerStatus, StatusRegistry.TradedFor);
        public bool StatusProtected => ReferenceEquals(PlayerStatus, StatusRegistry.Protected);
        public bool StatusSelected => ReferenceEquals(PlayerStatus, StatusRegistry.Selected);
        public bool StatusAvailable => ReferenceEquals(PlayerStatus, StatusRegistry.Available);
        public bool Clickable => StatusSelected || StatusAvailable;
        public bool IsSelected => StatusSelected || StatusTradedFor;
        public int Id => Player.Id;

        // Properties for XAML binding
        public Color BackgroundColor => PlayerStatus.BackgroundColor;
        public Color BorderColor => PlayerStatus.BorderColor;
        public string StatusText => PlayerStatus.StatusText;
        public Color StatusTextColor => PlayerStatus.StatusTextColor;
        public Color StatusBackgroundColor => PlayerStatus.StatusBackgroundColor;
        public string Icon => PlayerStatus.Icon;
        public Color IconColor => PlayerStatus.IconColor;
        public bool HasStatus => !string.IsNullOrEmpty(StatusText) && StatusText != "DISPONIBLE";
        public bool HasIcon => !string.IsNullOrEmpty(Icon);
        public bool CanRemove => StatusSelected && !StatusProtected;

        public PlayerModel(SelectableDto player, bool traded, bool prot, bool selected)
        {
            Player = player;
            PlayerStatus = traded ? StatusRegistry.TradedAway
                : prot && selected ? StatusRegistry.TradedFor
                : prot ? StatusRegistry.Protected
                : selected ? StatusRegistry.Selected
                : StatusRegistry.Available;
        }

        public void TogglePlayer()
        {
            if (StatusAvailable)
                PlayerStatus = StatusRegistry.Selected;
            else if (StatusSelected && !StatusProtected)
                PlayerStatus = StatusRegistry.Available;

            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(PlayerStatus));

            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusTextColor));
            OnPropertyChanged(nameof(StatusBackgroundColor));
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconColor));
        }
    }

    public partial class PositionGroupModel : ObservableObject
    {
        public string Position => Players?.FirstOrDefault()?.Player?.Position ?? string.Empty;
        public ObservableCollection<PlayerModel> Players { get; } = [];

        public PositionGroupModel(IList<PlayerModel> players)
        {
            foreach (PlayerModel player in players)
                Players.Add(player);
        }
    }

    public partial class FranchiseModel : ObservableObject
    {
        [ObservableProperty] private FranchiseInfo franchiseInfo;
        [ObservableProperty] private int selectedCount;
        [ObservableProperty] private int maxCount;
        [ObservableProperty] private Color backgroundColor;

        public string PlayerCountText => $"{SelectedCount}/{MaxCount}";
        public ObservableCollection<PositionGroupModel> PositionGroups { get; } = [];
        private int PlayerCount => PositionGroups.SelectMany(pg => pg.Players).Count();

        public FranchiseModel(FranchiseInfo info, int maxCount)
        {
            FranchiseInfo = info;
            MaxCount = maxCount;
            BackgroundColor = Color.FromArgb("#f8f9fa");
            SelectedCount = 0;
        }

        public FranchiseModel(FranchiseInfo info, int selectedCount, int maxCount) : this(info, maxCount)
        {
            SelectedCount = selectedCount;
        }
    }

    public partial class FormationPosition : ObservableObject
    {
        [ObservableProperty] private string positionId = "";
        [ObservableProperty] private string positionName = "";
        [ObservableProperty] private string requiredPosition = "";
        [ObservableProperty] private PlayerModel? assignedPlayer;
        [ObservableProperty] private string playerName = "Empty";
        [ObservableProperty] private Color playerBackgroundColor = Color.FromArgb("#dc3545");
        [ObservableProperty] private Color playerBorderColor = Colors.White;
        [ObservableProperty] private Rect layoutBounds;
        [ObservableProperty] private int playerIndex;

        public FormationPosition(string id, string name, string position, double x, double y, int index)
        {
            PositionId = id;
            PositionName = name;
            RequiredPosition = position;
            PlayerIndex = index;
            LayoutBounds = new Rect(x / 100.0, y / 100.0, 60, 60);
            UpdateAppearance();
        }

        public void AssignPlayer(PlayerModel? player)
        {
            AssignedPlayer = player;
            PlayerName = player?.Player.Name ?? "Empty";
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            if (AssignedPlayer != null)
            {
                PlayerBackgroundColor = Color.FromArgb("#28a745");
                PlayerBorderColor = Colors.White;
            }
            else
            {
                PlayerBackgroundColor = Color.FromArgb("#dc3545");
                PlayerBorderColor = Colors.White;
            }
        }
    }

    public partial class DraggablePlayer : ObservableObject
    {
        [ObservableProperty] private PlayerModel player;
        [ObservableProperty] private bool isEligible = true;

        public DraggablePlayer(PlayerModel player)
        {
            Player = player;
        }
    }

    public partial class FormationDisplayPosition : ObservableObject
    {
        [ObservableProperty] private string positionName;
        [ObservableProperty] private string playerName;
        [ObservableProperty] private Color playerBackgroundColor;
        [ObservableProperty] private Rect layoutBounds;

        public FormationDisplayPosition(string position, string player, double x, double y, bool hasPlayer)
        {
            PositionName = position;
            PlayerName = player;
            PlayerBackgroundColor = hasPlayer ? Color.FromArgb("#28a745") : Color.FromArgb("#dc3545");
            LayoutBounds = new Rect(x / 100.0, y / 100.0, 45, 45);
        }
    }
}