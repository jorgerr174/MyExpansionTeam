using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Create : ContentPage
    {
        public Create(CreateViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}