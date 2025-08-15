using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Trades : ContentPage
    {
        public Trades(TradesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TradesViewModel viewModel)
            {
                // Get teamId from navigation parameters
                // await viewModel.LoadTradesCommand.ExecuteAsync(teamId);
            }
        }
    }
}