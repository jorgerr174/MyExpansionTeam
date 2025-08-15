using MobileApp.Models.Account;

namespace MobileApp.Views.Account
{
    public partial class EditProfile : ContentPage
    {
        public EditProfile(EditProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}