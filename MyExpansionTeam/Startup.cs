/*
 * using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace METAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigurationServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "METAPI", Version = "v1" });
            });
            services.AddDbContext<PlayersContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        }
    }
}
*/