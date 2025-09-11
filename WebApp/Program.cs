using Microsoft.Extensions.Options;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();

            ConfigureHttpClient(services, configuration);

            ConfigureAuthentication(services);
            services.AddAuthorization();
        }

        static void ConfigureHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(Options.DefaultName, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);

                var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);

                client.DefaultRequestHeaders.Add("User-Agent", "WebApp/1.0");
            });


            var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            services.AddHttpClient("_fastClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);
            });

            services.AddHttpClient("_httpClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                if (!string.IsNullOrEmpty(apiBaseUrl)) client.BaseAddress = new Uri(apiBaseUrl);
            });

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
                    options.AccessDeniedPath = "/Home/Index";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;
                });
        }


        static void ConfigurePipeline(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }
    }
}