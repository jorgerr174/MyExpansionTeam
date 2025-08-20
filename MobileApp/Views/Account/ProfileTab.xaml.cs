using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class ProfileTab : ContentPage
    {
        public ProfileTab(ProfileTabViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ProfileTabViewModel viewModel)
            {
                await viewModel.LoadAuthStateCommand.ExecuteAsync(null);
            }
        }
    }
}