using METCore.DTOs.Player;
using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    public partial class Draft : ContentPage, ITeamBase<DraftViewModel>
    {
        public int TeamId { get; set; }

        public Draft(DraftViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DraftViewModel viewModel)
                await viewModel.LoadViewAsync(TeamId);
        }
    }
}