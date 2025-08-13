using CommunityToolkit.Mvvm.ComponentModel;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class SignUpViewModel(AccountService authService) : ObservableObject
    {
        private readonly AccountService _authService;
    }
}