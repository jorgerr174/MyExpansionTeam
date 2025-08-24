using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    public partial class Roster : ContentPage, ITeamBase<RosterViewModel>
    {
        public int TeamId { get; set; }

        public Roster(RosterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is RosterViewModel viewModel)
                viewModel.LoadViewAsync(TeamId);
        }

        private void OnPlayerDragStarting(object sender, DragStartingEventArgs e)
        {
            var border = sender as Border;
            var player = border?.BindingContext as DraggablePlayer;
            if (player != null)
            {
                e.Data.Properties["Player"] = player;
            }
        }

        private void OnPositionDrop(object sender, DropEventArgs e)
        {
            var border = sender as Border;
            var position = border?.BindingContext as FormationPosition;

            if (position != null && e.Data.Properties.TryGetValue("Player", out var playerObj) && playerObj is DraggablePlayer player)
            {
                ((RosterViewModel)BindingContext).DropPlayer(position, player);
            }
        }

        private void OnPositionTapped(object sender, EventArgs e)
        {
            var border = sender as Border;
            var position = border?.BindingContext as FormationPosition;
            if (position != null)
            {
                ((RosterViewModel)BindingContext).SelectPositionPlayer(position);
            }
        }
    }
}