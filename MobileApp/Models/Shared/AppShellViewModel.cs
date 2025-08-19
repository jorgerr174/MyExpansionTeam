using CommunityToolkit.Mvvm.ComponentModel;
using MobileApp.Services;

namespace MobileApp.Models.Shared
{
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly AccountService _accountService;

        public AppShellViewModel(AccountService authService)
        {
            _accountService = authService;
            checkLogIn();
        }

        [ObservableProperty] public bool isLoggedIn = false;

        public bool isNotLoggedIn => !IsLoggedIn;


        private async void checkLogIn()
        {
            IsLoggedIn = await _accountService.TryAutoLogInAsync();
            OnPropertyChanged(nameof(isNotLoggedIn));
        }
    }
}