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
    }
}