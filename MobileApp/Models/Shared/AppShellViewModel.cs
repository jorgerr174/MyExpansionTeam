using CommunityToolkit.Mvvm.ComponentModel;
using MobileApp.Services;

namespace MobileApp.Models.Shared
{
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly AccountService _accountService;

        public AppShellViewModel(AccountService accountService)
        {
            _accountService = accountService;
            checkLogIn();
        }

        [ObservableProperty] public bool isLoggedIn = false;

        public bool isNotLoggedIn => !IsLoggedIn;


        private async void checkLogIn()
        {
            IsLoggedIn = await AccountService.TryAutoLogInAsync();
            OnPropertyChanged(nameof(isNotLoggedIn));
        }
    }
}