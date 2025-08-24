using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "teamId")]
    [QueryProperty(nameof(CurrentPick), "currentPick")]
    public partial class Trade : ContentPage
    {
        public int TeamId { get; set; }
        public int CurrentPick { get; set; } = -1;

        public Trade(TradeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TradeViewModel viewModel)
                viewModel.LoadTrade(TeamId, CurrentPick);
        }
    }
}