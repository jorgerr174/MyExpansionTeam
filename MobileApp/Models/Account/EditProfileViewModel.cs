using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class EditProfileViewModel(AccountService accountService) : BaseViewModel
    {
        private readonly AccountService _accountService = accountService;


        [RelayCommand] public static async Task GoBack() => await BaseService.GoBackAsync(null);
        [RelayCommand] public static async Task GoToUpdateUser() => await BaseService.GoToAsync(AppRoutes.UpdateUser, null);
        [RelayCommand] public static async Task GoToUpdateCredentials() => await BaseService.GoToAsync(AppRoutes.UpdateCredentials, null);
    }
}