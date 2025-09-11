using System.Text;
using AutoMapper;
using METCore.Interfaces;
using METCore.Services;
using METDAL.Data;
using METDAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace METAPI
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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                .UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("METDAL")
                )
                .UseLazyLoadingProxies();
            });

            services.AddAutoMapper(typeof(METCore.Mapping.MappingProfile));

            ConfigureAuthentication(services, configuration);
            services.AddAuthorization();

            services.AddControllers();

            RegisterApplicationServices(services);
        }


        static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Consider setting to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ??
                        throw new InvalidOperationException("JWT SecretKey is not configured")))
                };
            });
        }


        static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<ImportService>();
            services.AddScoped<UserService>();
            services.AddScoped<FranchiseService>();
            services.AddScoped<TeamService>();
            services.AddScoped<PlayerService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFranchiseRepository, FranchiseRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITradeRepository, TradeRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ISeasonStatsRepository, SeasonStatsRepository>();
        }


        static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using var scope = app.Services.CreateScope();
                scope.ServiceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
                Console.WriteLine("AutoMapper configuration is valid!");
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}



