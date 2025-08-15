using MobileApp.Models.Shared;
using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class RosterSettings : ContentPage
    {
        public RosterSettings(RosterSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Get teamId from query parameters and load settings
            if (BindingContext is RosterSettingsViewModel viewModel)
            {
                // This would typically be passed via Shell navigation parameters
                // For now, you'll need to implement parameter passing
                // await viewModel.LoadRosterSettingsCommand.ExecuteAsync(teamId);
            }
        }

        private void OnPlayerSelectionChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is SelectablePlayerViewModel playerWrapper)
            {
                if (BindingContext is RosterSettingsViewModel viewModel)
                {
                    viewModel.TogglePlayerSelectionCommand.Execute(playerWrapper);
                }
            }
        }
    }
}