using CommunityToolkit.Mvvm.ComponentModel;
using MobileApp.Services;

namespace MobileApp.Models.Shared
{
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly AccountService _authService;

        public AppShellViewModel(AccountService authService)
        {
            _authService = authService;
            checkLogIn();
        }

        [ObservableProperty] public bool isLoggedIn = false;

        public bool isNotLoggedIn => !IsLoggedIn;


        private async void checkLogIn()
        {
            IsLoggedIn = await _authService.TryAutoLogInAsync();
            OnPropertyChanged(nameof(isNotLoggedIn));
        }
    }
}