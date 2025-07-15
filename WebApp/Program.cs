using Microsoft.Extensions.Options;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure middleware pipeline
            ConfigurePipeline(app);

            app.Run();
        }

        static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // MVC Services
            services.AddControllersWithViews();

            // HTTP Client with custom timeout
            ConfigureHttpClient(services, configuration);

            // Authentication & Authorization
            ConfigureAuthentication(services);
            services.AddAuthorization();
        }

        static void ConfigureHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            // Configure default HttpClient with 3-minute timeout
            services.AddHttpClient(Options.DefaultName, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);

                // Optional: Set base address for your API
                var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);

                // Optional: Default headers
                client.DefaultRequestHeaders.Add("User-Agent", "WebApp/1.0");
            });

            // Multiple named HttpClients with different timeouts for different purposes

            // Fast operations (1 minute)
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            services.AddHttpClient("_fastClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Standard API calls (3 minutes)
            services.AddHttpClient("_httpClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Long-running operations (5 minutes)
            services.AddHttpClient("_importClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);
            });
        }


        static void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/LogIn";
                    options.LogoutPath = "/Account/LogOut";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(24); // Optional: Set cookie expiration
                    options.SlidingExpiration = true; // Optional: Extend expiration on activity
                });
        }


        static void ConfigurePipeline(WebApplication app)
        {
            // Error handling and security (production)
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // 30 days default - consider customizing for production
            }

            // Core middleware
            app.UseHttpsRedirection();
            app.UseRouting();

            // Authentication & Authorization
            app.UseAuthentication(); // Add this - it was missing!
            app.UseAuthorization();

            // Static files and routing
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }
    }
}