
using System.Reflection;

namespace Waslah.Presistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor) 
        : IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
       
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            var entries = ChangeTracker.Entries<AuditableEntity>();

            foreach (var entityEntery in entries)
            {
                var CurrentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();

                if (entityEntery.State == EntityState.Added)
                    entityEntery.Property(x => x.CreatedById).CurrentValue = CurrentUserId!;

                if (entityEntery.State == EntityState.Modified)
                {
                    entityEntery.Property(x => x.UpdatedById).CurrentValue = CurrentUserId;
                    entityEntery.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<Station> Stations { get; set; }
        public DbSet<MyRoute> MyRoutes { get; set; }
        public DbSet<ChainedRoute> ChainedRoutes { get; set; }
        public DbSet<RouteConnector> RouteConnectors { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserPoints> UserPoints { get; set; }
        public DbSet<ModelEncodingMap> EncodingMaps { get; set; }
        public DbSet<OtpEntry> OtpEntries { get; set; }
        public DbSet<AgencyTrip> AgenciesTrip { get; set; }
        public DbSet<AgencyLink> Links { get; set; }

    }
}
