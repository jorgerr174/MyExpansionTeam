using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class SignUp : ContentPage
    {
        public SignUp(SignUpViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}