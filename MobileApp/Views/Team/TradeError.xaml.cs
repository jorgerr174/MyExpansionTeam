using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class TradeError : ContentPage
    {
        public TradeError(TradeErrorViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}