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
            checkLogin();
        }

        [ObservableProperty] public bool isLoggedIn = false;

        public bool isNotLoggedIn => !IsLoggedIn;


        private async void checkLogin()
        {
            IsLoggedIn = await _authService.TryAutoLoginAsync();
            OnPropertyChanged(nameof(isNotLoggedIn));
        }
    }
}