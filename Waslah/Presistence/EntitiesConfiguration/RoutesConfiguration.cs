
namespace Waslah.Presistence.EntitiesConfiguration
{
    public class RoutesConfiguration : IEntityTypeConfiguration<MyRoute>
    {
        public void Configure(EntityTypeBuilder<MyRoute> builder)
        {
            builder.HasKey(R => R.Id);
            builder.ToTable("MyRoutes");

            builder.
                HasOne(R => R.PriStation)
                .WithMany(s => s.GoingRoute)
                .HasForeignKey(R => R.PrimaryLocId)
                .HasPrincipalKey(s => s.LocationId);

            builder.
                HasOne(R => R.SecStation)
               .WithMany(s => s.ReturnedRoute)
               .HasForeignKey(R => R.SecondaryLocId)
               .HasPrincipalKey(s => s.LocationId);

            builder
            .HasMany(r => r.chainedRoutes)
            .WithMany(cr => cr.Routes)
            .UsingEntity<RouteConnector>(
        x => x
            .HasOne(rc => rc.ChainedRoute)
            .WithMany(r => r.RoutesInfo)
            .HasForeignKey(rc => rc.ChainedRouteID),
        y => y
            .HasOne(rc => rc.MyRoute)
            .WithMany(r => r.connectors)
            .HasForeignKey(rc => rc.MyRouteId),
        j => j
            .HasKey(k => new { k.MyRouteId, k.ChainedRouteID })
    );
        }
    }
}
