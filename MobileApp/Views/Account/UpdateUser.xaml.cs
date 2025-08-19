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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is UpdateUserViewModel viewModel)
                await viewModel.LoadProfileCommand.ExecuteAsync(null);
        }
    }
}