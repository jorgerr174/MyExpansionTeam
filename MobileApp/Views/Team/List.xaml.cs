using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class List : ContentPage
    {
        public List(ListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ListViewModel viewModel)
                await viewModel.LoadTeamsCommand.ExecuteAsync(null);
        }
    }
}