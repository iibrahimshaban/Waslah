
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Waslah.Authentication;
using Waslah.Authentication.Filters;
using Waslah.Settings;

namespace Waslah
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddDependancies(this IServiceCollection services 
            ,IConfiguration configuration)
        {
            services.AddControllers();
            services
                .AddOpenApi()
                .AddDbContextConfig(configuration)
                .AddServicesRegisteration()
                .AddMappingConfig()
                .AddIdentityConfiguration(configuration)
                .AddBackGroundJobsConfig(configuration);

            return services;
        }

        private static IServiceCollection AddDbContextConfig(this IServiceCollection services
            ,IConfiguration configuration) 
        {
            var ConnectionSting = configuration.GetConnectionString("DefaultConnection") ??
               throw new InvalidOperationException("connection string 'ConnectionSting' not found");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionSting));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            return services;
        }
        private static IServiceCollection AddServicesRegisteration(this IServiceCollection services)
        {
            services.AddScoped<IStationServices, StationServices>();
            services.AddScoped<IRouteGeneratorServices,RouteGeneratorServices>();
            services.AddScoped<IChainedRouteServices,ChainedRouteServices>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomEmailService,EmailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAgencyTripService, AgencyTripService>();

            services.AddHybridCache();
            services.AddHttpContextAccessor();

            return services;
        }
        private static IServiceCollection AddMappingConfig(this IServiceCollection services)
        {
            var MappingConfig = TypeAdapterConfig.GlobalSettings;
            MappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(MappingConfig));

            services.AddFluentValidationAutoValidation()
             .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
        private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services
    ,       IConfiguration configuration)
        {

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


            services.AddScoped<IAuthService, AuthService>();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            services.AddOptions<JwtOptions>()
                    .BindConfiguration(JwtOptions.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

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

            return services;

        }
        private static IServiceCollection AddBackGroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Hangfire services.
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            return services;
        }
    }
    
}
