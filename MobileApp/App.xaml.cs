using MobileApp.Services;
using MobileApp.Views.Shared;

namespace MobileApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell(new Models.Shared.AppShellViewModel(Handler?.MauiContext?.Services.GetService<AccountService>()));
    }

    protected override async void OnResume()
    {
        base.OnResume();

        try
        {
            if (Handler?.MauiContext?.Services.GetService<AccountService>() is AccountService accountService)
            {
                bool isAuthenticated = await BaseService.IsAuthenticatedAsync();
            }
        }
        catch (Exception ex) { }
    }
}