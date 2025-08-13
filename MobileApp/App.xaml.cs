using MobileApp.Services;

namespace MobileApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    // 🎯 EXACT equivalent of WebApp startup behavior
    protected override async void OnStart()
    {
        base.OnStart();

        // Just like WebApp starts with /Home/Index regardless of auth state
        // The Home/Index page will handle auth checking internally (same as WebApp)
        await Shell.Current.GoToAsync("Home/Index");
    }

    // Handle app sleep/resume for token validation
    protected override async void OnSleep()
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
                bool isAuthenticated = await accountService.IsAuthenticatedAsync();

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