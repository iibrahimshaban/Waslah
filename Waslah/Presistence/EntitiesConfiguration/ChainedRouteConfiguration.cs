
namespace Waslah.Presistence.EntitiesConfiguration
{
    public class ChainedRouteConfiguration : IEntityTypeConfiguration<ChainedRoute>
    {
        public void Configure(EntityTypeBuilder<ChainedRoute> builder)
        {
            builder.HasIndex(x => new { x.FirstLocId, x.LastLocId, x.NumberOfRides }).IsUnique();

            builder
                .HasOne(x => x.FirstLoc)
                .WithMany(x => x.StartChainedRoutes)
                .HasForeignKey(x => x.FirstLocId)
                .HasPrincipalKey(x => x.LocationId);

            builder
                .HasOne(x => x.LastLoc)
                .WithMany(x => x.EndChainedRoutes)
                .HasForeignKey(x => x.LastLocId)
                .HasPrincipalKey(x => x.LocationId);
        }
    }
}
