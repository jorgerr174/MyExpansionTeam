using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Views.Shared
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            AccountService.UserLoggedOut += OnUserLoggedOut;
        }

        private async void OnUserLoggedOut(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HomeTab");
            await Shell.Current.GoToAsync("//MyTeamsTab");
            await Shell.Current.GoToAsync("//ProfileTab");
        }
    }
}