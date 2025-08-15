using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class ReviewRoster : ContentPage
    {
        public ReviewRoster(ReviewRosterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ReviewRosterViewModel viewModel)
            {
                // Get teamId from navigation parameters
                // await viewModel.LoadRosterCommand.ExecuteAsync(teamId);
            }
        }
    }
}