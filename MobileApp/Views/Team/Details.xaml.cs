using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Details : ContentPage
    {
        public Details(DetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is DetailsViewModel viewModel)
            {
                if (GetTeamIdFromParameters() is int teamId && teamId > 0)
                    await viewModel.LoadTeamDetailsCommand.ExecuteAsync(teamId);
                else
                {
                    viewModel.HasLoadError = true;
                    viewModel.LoadErrorMessage = "No team ID provided";
                }
            }
        }

        private int GetTeamIdFromParameters()
        {
            var query = Shell.Current.CurrentState.Location.Query;
            return 1;
        }
    }
}