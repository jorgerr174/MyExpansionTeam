using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    public partial class Details : ContentPage, ITeamBase<DetailsViewModel>
    {
        public int TeamId { get; set; }

        public Details(DetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DetailsViewModel viewModel)
                viewModel.LoadViewAsync(TeamId);
        }
    }
}