
using System.Reflection;

namespace Waslah.Presistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : IdentityDbContext(options)
       
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var CascadedFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(x => x.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

            foreach (var foreignKey in CascadedFKs)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Station> Stations { get; set; }
        public DbSet<MyRoute> MyRoutes { get; set; }
        public DbSet<ChainedRoute> ChainedRoutes { get; set; }
        public DbSet<RouteConnector> RouteConnectors { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserPoints> UserPoints { get; set; }

    }
}
