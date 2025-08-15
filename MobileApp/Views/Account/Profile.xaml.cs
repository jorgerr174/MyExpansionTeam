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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ProfileViewModel viewModel)
            {
                await viewModel.LoadProfileCommand.ExecuteAsync(null);
            }
        }
    }
}