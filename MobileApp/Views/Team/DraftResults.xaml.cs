using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class DraftResults : ContentPage
    {
        public DraftResults(DraftResultsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DraftResultsViewModel viewModel)
            {
                // Get teamId from navigation parameters
                // await viewModel.LoadDraftResultsCommand.ExecuteAsync(teamId);
            }
        }
    }
}