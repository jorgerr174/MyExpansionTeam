using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class Edit : ContentPage
    {
        public Edit(EditViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}