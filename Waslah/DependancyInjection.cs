
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Waslah.Authentication;

namespace Waslah
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddDependancies(this IServiceCollection services 
            ,IConfiguration configuration)
        {
            services.AddControllers();
            services
                .AddIdentityConfiguration(configuration)
                .AddOpenApi()
                .AddDbContextConfig(configuration)
                .AddServicesRegisteration()
                .AddMappingConfig();

            return services;
        }

        private static IServiceCollection AddDbContextConfig(this IServiceCollection services
            ,IConfiguration configuration) 
        {
            var ConnectionSting = configuration.GetConnectionString("DefaultConnection") ??
               throw new InvalidOperationException("connection string 'ConnectionSting' not found");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionSting));

            return services;
        }
        private static IServiceCollection AddServicesRegisteration(this IServiceCollection services)
        {
            services.AddScoped<IStationServices, StationServices>();
            services.AddScoped<IRouteGeneratorServices,RouteGeneratorServices>();
            services.AddScoped<IChainedRouteServices,ChainedRouteServices>();
            services.AddScoped<IOrderService, OrderService>();
            
            return services;
        }
        private static IServiceCollection AddMappingConfig(this IServiceCollection services)
        {
            var MappingConfig = TypeAdapterConfig.GlobalSettings;
            MappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(MappingConfig));

            services.AddFluentValidationAutoValidation()
             .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
        private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services
    ,       IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(o =>
             {
                 o.SaveToken = true;
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     IssuerSigningKey = new
                     SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings!.Key)),
                     ValidIssuer = settings.Issuer,
                     ValidAudience = settings.Audience
                 };
             });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            });

            return services;

        }
    }
    
}
