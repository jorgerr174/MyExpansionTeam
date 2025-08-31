using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
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
                viewModel.LoadViewAsync(TeamId);
        }
    }
}