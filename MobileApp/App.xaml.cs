using MobileApp.Services;
using MobileApp.Views.Shared;

namespace MobileApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell(new Models.Shared.AppShellViewModel(Handler?.MauiContext?.Services.GetService<AccountService>())));
    }

    // 🎯 EXACT equivalent of WebApp startup behavior
    protected override void OnStart()
    {
        base.OnStart();
    }

    // Handle app sleep/resume for token validation
    protected override void OnSleep()
    {
        base.OnSleep();
        // App going to sleep - could save state here if needed
    }

    protected override async void OnResume()
    {
        base.OnResume();

        // App resuming - check if token is still valid
        try
        {
            var accountService = Handler?.MauiContext?.Services.GetService<AccountService>();
            if (accountService != null)
            {
                bool isAuthenticated = await BaseService.IsAuthenticatedAsync();

                // If token expired, user will see login prompts in UI
                // No need to force navigation - let each page handle its own auth state
                // This matches WebApp behavior where expired auth is handled per-page
            }
        }
        catch
        {
            // Ignore errors on resume - let individual pages handle auth
        }
    }
}