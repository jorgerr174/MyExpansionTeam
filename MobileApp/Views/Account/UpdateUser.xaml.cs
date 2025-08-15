using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class UpdateUser : ContentPage
    {
        public UpdateUser(UpdateUserViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}