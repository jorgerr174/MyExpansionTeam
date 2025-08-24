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

        private void OnPositionDrop(object sender, DropEventArgs e)
        {
            if (BindingContext is RosterViewModel vm && sender is Border border && border.BindingContext is FormationPosition position)
                vm.DropPlayerCommand.Execute(position);
        }

        private void OnPlayerDragStarting(object sender, DragStartingEventArgs e)
        {
            if (BindingContext is RosterViewModel vm && sender is Border border && border.BindingContext is DraggablePlayer player)
                vm.StartDragCommand.Execute(player);
        }
    }
}