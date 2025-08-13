using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class LogIn : ContentPage
    {
        public LogIn(LogInViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}