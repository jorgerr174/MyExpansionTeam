using CommunityToolkit.Mvvm.ComponentModel;
using METCore.DTOs.Player;

namespace MobileApp.Models.Shared
{
    public partial class SelectablePlayerViewModel : ObservableObject
    {
        public SelectablePlayerViewModel(SelectableDto player)
        {
            Player = player;
            IsSelected = false;
        }

        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private bool isAlreadyRostered = false;

        public bool CanBeToggled => !IsAlreadyRostered;

        public SelectableDto Player { get; }
        public int Id => Player.Id;
        public string Name => Player.Name;
        public string Position => Player.Position;
        public string APY => Player.APY;
        public string Height => Player.Height;
        public string Weight => Player.Weight;
        public static string TeamName => ""; // This would need to be populated from somewhere
    }
}