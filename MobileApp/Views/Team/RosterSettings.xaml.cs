using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    public partial class RosterSettings : ContentPage, ITeamBase<RosterSettingsViewModel>
    {
        public int TeamId { get; set; }

        public RosterSettings(RosterSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is RosterSettingsViewModel viewModel)
                await viewModel.LoadViewAsync(TeamId);
        }
    }
}