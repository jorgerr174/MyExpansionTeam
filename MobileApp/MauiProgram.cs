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


            builder.Services.AddHttpClient("_fastClient", client =>
            {
                client.BaseAddress = new Uri(MauiSettings.apiURL);
                client.Timeout = TimeSpan.FromMinutes(1);
            });

            builder.Services.AddHttpClient("_httpClient", client =>
            {
                client.BaseAddress = new Uri(MauiSettings.apiURL);
                client.Timeout = TimeSpan.FromMinutes(3);
            });

            #region Services
            builder.Services.AddSingleton<HomeService>();
            builder.Services.AddSingleton<AccountService>();
            builder.Services.AddTransient<TeamService>();
            #endregion Services

            #region Views
            #region Views.Account
            builder.Services.AddTransient<Views.Account.EditProfile>();
            builder.Services.AddTransient<Views.Account.LogIn>();
            builder.Services.AddTransient<Views.Account.Profile>();
            builder.Services.AddTransient<Views.Account.SignUp>();
            builder.Services.AddTransient<Views.Account.UpdateCredentials>();
            builder.Services.AddTransient<Views.Account.UpdateUser>();
            #endregion Views.Account

            #region Views.Home
            builder.Services.AddTransient<Views.Home.Index>();
            #endregion Views.Home

            #region Views.Shared
            builder.Services.AddSingleton<Views.Shared.AppShell>();
            builder.Services.AddSingleton<Views.Shared.Error>();
            #endregion Views.Shared

            #region Views.Team
            builder.Services.AddTransient<Views.Team.Create>();
            builder.Services.AddTransient<Views.Team.Details>();
            builder.Services.AddTransient<Views.Team.Draft>();
            builder.Services.AddTransient<Views.Team.Edit>();
            builder.Services.AddTransient<Views.Team.List>();
            builder.Services.AddTransient<Views.Team.MyTeams>();
            builder.Services.AddTransient<Views.Team.Roster>();
            builder.Services.AddTransient<Views.Team.RosterSettings>();
            builder.Services.AddTransient<Views.Team.Trade>();
            builder.Services.AddTransient<Views.Team.TradeError>();
            #endregion Views.Team
            #endregion Views

            #region Models
            #region Models.Account
            builder.Services.AddTransient<Models.Account.EditProfileViewModel>();
            builder.Services.AddTransient<Models.Account.LogInViewModel>();
            builder.Services.AddTransient<Models.Account.ProfileViewModel>();
            builder.Services.AddTransient<Models.Account.SignUpViewModel>();
            builder.Services.AddTransient<Models.Account.UpdateCredentialsViewModel>();
            builder.Services.AddTransient<Models.Account.UpdateUserViewModel>();
            #endregion Models.Account

            #region Models.Home
            builder.Services.AddTransient<Models.Home.IndexViewModel>();
            #endregion Models.Home

            #region Models.Shared
            builder.Services.AddSingleton<Models.Shared.AppShellViewModel>();
            builder.Services.AddSingleton<Models.Shared.ErrorViewModel>();
            builder.Services.AddTransient<Models.Shared.SelectablePlayerViewModel>();
            #endregion Models.Shared

            #region Models.Team
            builder.Services.AddTransient<Models.Team.CreateViewModel>();
            builder.Services.AddTransient<Models.Team.DetailsViewModel>();
            builder.Services.AddTransient<Models.Team.DraftViewModel>();
            builder.Services.AddTransient<Models.Team.DraftResultsViewModel>();
            builder.Services.AddTransient<Models.Team.EditViewModel>();
            builder.Services.AddTransient<Models.Team.FormationViewModel>();
            builder.Services.AddTransient<Models.Team.ListViewModel>();
            builder.Services.AddTransient<Models.Team.MyTeamsViewModel>();
            builder.Services.AddTransient<Models.Team.ReviewRosterViewModel>();
            builder.Services.AddTransient<Models.Team.RosterViewModel>();
            builder.Services.AddTransient<Models.Team.RosterSettingsViewModel>();
            builder.Services.AddTransient<Models.Team.TradeViewModel>();
            builder.Services.AddTransient<Models.Team.TradeErrorViewModel>();
            builder.Services.AddTransient<Models.Team.TradesViewModel>();
            #endregion Models.Team
            #endregion Models


            return builder.Build();
        }
    }

    public static class MauiSettings
    {
        public static string apiURL = "https://localhost:7087/api/";
    }
}
