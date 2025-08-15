using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "teamId")]
    [QueryProperty(nameof(Context), "context")]
    [QueryProperty(nameof(CurrentPick), "currentPick")]

    public partial class Trade : ContentPage
    {
        public int TeamId { get; set; }
        public string Context { get; set; } = "roster";
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
            {
                await viewModel.InitializeTradeAsync(TeamId, Context, CurrentPick);
            }
        }
    }
}