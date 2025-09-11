using CommunityToolkit.Mvvm.ComponentModel;
using METCore.DTOs.Player;

namespace MobileApp.Models.Team
{
    public partial class ProtectedPlayerViewModel : ObservableObject
    {
        public ProtectedPlayerViewModel(ProtectableDto player)
        {
            Player = player;
            IsProtected = player.DefaultProtected;
        }

        [ObservableProperty] private bool isProtected;

        public ProtectableDto Player { get; }
    }
}