namespace MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("Home/Index", typeof(Views.Home.Index));
            Routing.RegisterRoute("Account/LogIn", typeof(Views.Account.LogIn));

            //Routing.RegisterRoute("Team/MyTeams", typeof(Index));
            //Routing.RegisterRoute("Team/Create", typeof(Index));  
            //Routing.RegisterRoute("Admin/Index", typeof(Index));
        }
    }
}
