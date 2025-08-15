using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class UpdateUserViewModel : BaseViewModel
    {
        private readonly AccountService _accountService;

        public UpdateUserViewModel(AccountService accountService)
        {
            _accountService = accountService;
        }
    }
}