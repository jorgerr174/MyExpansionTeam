using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class Profile : ContentPage
    {
        public Profile(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}