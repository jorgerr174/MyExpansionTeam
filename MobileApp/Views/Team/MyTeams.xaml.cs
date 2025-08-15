using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class MyTeams : ContentPage
    {
        public MyTeams(MyTeamsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MyTeamsViewModel viewModel)
                await viewModel.LoadTeamsCommand.ExecuteAsync(null);
        }
    }
}