using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class EditProfileViewModel : BaseViewModel
    {
        private readonly AccountService _authService;

        public EditProfileViewModel(AccountService authService)
        {
            _authService = authService;
        }
    }
}