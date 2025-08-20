using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Details : ContentPage
    {        
        public Details(DetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}