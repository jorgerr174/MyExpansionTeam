using MobileApp.Models.Admin;

namespace MobileApp.Views.Admin
{
    public partial class Admin : ContentPage
    {
        public Admin(AdminViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}