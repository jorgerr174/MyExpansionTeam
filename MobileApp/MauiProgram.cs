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


            #region HttpClients
            builder.Services.AddHttpClient("_fastClient", client =>
            {
                client.BaseAddress = new Uri(MauiSettings.apiURL);
                client.Timeout = TimeSpan.FromMinutes(1);
            })
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if ANDROID
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return handler;
            })
#endif
            ;
            builder.Services.AddHttpClient("_httpClient", client =>
            {
                client.BaseAddress = new Uri(MauiSettings.apiURL);
                client.Timeout = TimeSpan.FromMinutes(3);
            })
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if ANDROID
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return handler;
            })
#endif
            ;
            builder.Services.AddHttpClient("_importClient", client =>
            {
                client.BaseAddress = new Uri(MauiSettings.apiURL);
                client.Timeout = TimeSpan.FromMinutes(10);
            })
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if ANDROID
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return handler;
            })
#endif
            ;
            #endregion HttpClients


            #region Routing
            #region Tabs
            Routing.RegisterRoute(AppRoutes.HomeTab, typeof(Views.Home.Index));
            Routing.RegisterRoute(AppRoutes.MyTeamsTab, typeof(Views.Team.MyTeams));
            Routing.RegisterRoute(AppRoutes.ProfileTab, typeof(Views.Account.ProfileTab));
            #endregion Tabs

            #region Views.Account
            Routing.RegisterRoute(AppRoutes.EditProfile, typeof(Views.Account.EditProfile));
            Routing.RegisterRoute(AppRoutes.SignUp, typeof(Views.Account.SignUp));
            Routing.RegisterRoute(AppRoutes.UpdateCredentials, typeof(Views.Account.UpdateCredentials));
            Routing.RegisterRoute(AppRoutes.UpdateUser, typeof(Views.Account.UpdateUser));
            #endregion Views.Account

            #region Views.Admin
            Routing.RegisterRoute(AppRoutes.Admin, typeof(Views.Admin.Admin));
            Routing.RegisterRoute(AppRoutes.AssignRoles, typeof(Views.Admin.AssignRoles));
            Routing.RegisterRoute(AppRoutes.Import, typeof(Views.Admin.Import));
            #endregion Views.Admin

            #region Views.Team
            Routing.RegisterRoute(AppRoutes.CreateTeam, typeof(Views.Team.Create));
            Routing.RegisterRoute(AppRoutes.TeamDetails, typeof(Views.Team.Details));
            Routing.RegisterRoute(AppRoutes.DraftResults, typeof(Views.Team.DraftResults));
            Routing.RegisterRoute(AppRoutes.EditTeam, typeof(Views.Team.Edit));
            Routing.RegisterRoute(AppRoutes.Formation, typeof(Views.Team.Formation));
            Routing.RegisterRoute(AppRoutes.ReviewRoster, typeof(Views.Team.ReviewRoster));
            Routing.RegisterRoute(AppRoutes.Roster, typeof(Views.Team.Roster));
            Routing.RegisterRoute(AppRoutes.RosterSettings, typeof(Views.Team.RosterSettings));
            Routing.RegisterRoute(AppRoutes.Trade, typeof(Views.Team.Trade));
            Routing.RegisterRoute(AppRoutes.Trades, typeof(Views.Team.Trades));
            #endregion Views.Team
            #endregion Routing


            #region Services
            builder.Services.AddSingleton<AccountService>();
            builder.Services.AddSingleton<AdminService>();
            builder.Services.AddSingleton<HomeService>();
            builder.Services.AddSingleton<TeamService>();
            #endregion Services


            #region Views
            builder.Services.AddTransient<Views.Account.ProfileTab>();

            #region Views.Account
            builder.Services.AddTransient<Views.Account.EditProfile>();
            builder.Services.AddTransient<Views.Account.SignUp>();
            builder.Services.AddTransient<Views.Account.UpdateCredentials>();
            builder.Services.AddTransient<Views.Account.UpdateUser>();
            #endregion Views.Account

            #region Views.Admin
            builder.Services.AddTransient<Views.Admin.Admin>();
            builder.Services.AddTransient<Views.Admin.AssignRoles>();
            builder.Services.AddTransient<Views.Admin.Import>();
            #endregion Views.Admin

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
            builder.Services.AddTransient<Views.Team.DraftResults>();
            builder.Services.AddTransient<Views.Team.Edit>();
            //builder.Services.AddTransient<Views.Team.List>();
            builder.Services.AddTransient<Views.Team.MyTeams>();
            builder.Services.AddTransient<Views.Team.Roster>();
            builder.Services.AddTransient<Views.Team.RosterSettings>();
            builder.Services.AddTransient<Views.Team.Trade>();
            builder.Services.AddTransient<Views.Team.TradeError>();
            builder.Services.AddTransient<Views.Team.Trades>();
            #endregion Views.Team
            #endregion Views


            #region Models
            builder.Services.AddTransient<Models.Account.ProfileTabViewModel>();

            #region Models.Account
            builder.Services.AddTransient<Models.Account.EditProfileViewModel>();
            builder.Services.AddTransient<Models.Account.SignUpViewModel>();
            builder.Services.AddTransient<Models.Account.UpdateCredentialsViewModel>();
            builder.Services.AddTransient<Models.Account.UpdateUserViewModel>();
            #endregion Models.Account

            #region Models.Admin
            builder.Services.AddTransient<Models.Admin.AdminViewModel>();
            builder.Services.AddTransient<Models.Admin.AssignRolesViewModel>();
            builder.Services.AddTransient<Models.Admin.ImportViewModel>();
            #endregion Models.Admin

            #region Models.Home
            builder.Services.AddTransient<Models.Home.IndexViewModel>();
            #endregion Models.Home

            #region Models.Shared
            builder.Services.AddSingleton<Models.Shared.AppShellViewModel>();
            builder.Services.AddSingleton<Models.Shared.ErrorViewModel>();
            #endregion Models.Shared

            #region Models.Team
            builder.Services.AddTransient<Models.Team.CreateViewModel>();
            builder.Services.AddTransient<Models.Team.DetailsViewModel>();
            builder.Services.AddTransient<Models.Team.DraftViewModel>();
            builder.Services.AddTransient<Models.Team.DraftResultsViewModel>();
            builder.Services.AddTransient<Models.Team.EditViewModel>();
            builder.Services.AddTransient<Models.Team.FormationViewModel>();
            //builder.Services.AddTransient<Models.Team.ListViewModel>();
            builder.Services.AddTransient<Models.Team.MyTeamsViewModel>();
            //builder.Services.AddTransient<Models.Team.ReviewRosterViewModel>();
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

#if DEBUG
#if ANDROID
        public static string apiURL = "https://10.0.2.2:7087/api/";
#else
        public static string apiURL = "https://localhost:7087/api/";
#endif
#else
        public static string apiURL = "https://192.168.1.39:7087/api/";
#endif
    }
}
