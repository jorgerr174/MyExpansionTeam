using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class UpdateCredentials : ContentPage
    {
        public UpdateCredentials(UpdateCredentialsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}