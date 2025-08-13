using Microsoft.CodeAnalysis.CSharp.Syntax;
using MobileApp.Services;

namespace MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

            builder.Services.AddHttpClient<HomeService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7071/api/");
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            builder.Services.AddHttpClient<AccountService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7071/api/");
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            builder.Services.AddSingleton<HomeService>();
            builder.Services.AddSingleton<AccountService>();

            builder.Services.AddTransient<Views.Home.Index>();
            builder.Services.AddTransient<Views.Account.LogIn>();
            builder.Services.AddTransient<Models.Account.LogInViewModel>();

            return builder.Build();
        }
    }
}
