using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Formation : ContentPage
    {
        public Formation(FormationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is FormationViewModel viewModel)
            {
                // Get teamId from navigation parameters
                // await viewModel.LoadFormationCommand.ExecuteAsync(teamId);
            }
        }
    }
}