using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class EditProfileViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;

        [RelayCommand] public async Task GoToUpdateUser() => await _accountService.GoToAsync(AppRoutes.UpdateUser, null);
        [RelayCommand] public async Task GoToUpdateCredentials() => await _accountService.GoToAsync(AppRoutes.UpdateCredentials, null);
        [RelayCommand] public async Task GoBack() => await _accountService.GoBackAsync(null);
    }
}