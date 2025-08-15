using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Roster : ContentPage
    {
        public Roster(RosterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is RosterViewModel viewModel)
            {
                // Get teamId from navigation parameters
                // await viewModel.LoadRosterCommand.ExecuteAsync(teamId);
            }
        }
    }
}