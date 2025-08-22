using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "teamId")]
    public partial class Edit : ContentPage, ITeamBase<EditViewModel>
    {
        public int TeamId { get; set; }

        public Edit(EditViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EditViewModel viewModel)
                await viewModel.LoadViewAsync(TeamId);
        }
    }
}