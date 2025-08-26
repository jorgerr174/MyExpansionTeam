using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    [QueryProperty(nameof(FranchiseId), "FranchiseId")]
    [QueryProperty(nameof(TeamPicks), "TeamPicks")]
    [QueryProperty(nameof(FranchisePicks), "FranchisePicks")]
    public partial class Draft : ContentPage, ITeamBase<DraftViewModel>
    {
        public int TeamId { get; set; }
        public int FranchiseId { get; set; }
        public List<int> TeamPicks { get; set; } = [];
        public List<int> FranchisePicks { get; set; } = [];

        public Draft(DraftViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DraftViewModel viewModel)
                if (FranchiseId > 0) viewModel.HandleTradeReturn(FranchiseId, TeamPicks, FranchisePicks);
                else await viewModel.LoadViewAsync(TeamId);
        }
    }
}