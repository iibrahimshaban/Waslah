
namespace Waslah
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddDependancies(this IServiceCollection services 
            ,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi()
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
            services.AddScoped<IDistanceCalculator,DistanceCalculator>();
            services.AddScoped<IRouteGeneratorServices,RouteGeneratorServices>();
            services.AddScoped<IChainedRouteServices,ChainedRouteServices>();

            return services;
        }
        private static IServiceCollection AddMappingConfig(this IServiceCollection services)
        {
            var MappingConfig = TypeAdapterConfig.GlobalSettings;
            MappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(MappingConfig));

            return services;
        }
    }
    
}
