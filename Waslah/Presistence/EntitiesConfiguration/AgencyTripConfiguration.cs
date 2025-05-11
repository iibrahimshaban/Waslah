
namespace Waslah.Presistence.EntitiesConfiguration;

public class AgencyTripConfiguration : IEntityTypeConfiguration<AgencyTrip>
{
    public void Configure(EntityTypeBuilder<AgencyTrip> builder)
    {
        builder.Property(x => x.AgencyName).HasMaxLength(100);
        builder.Property(x => x.PickUpCity).HasMaxLength(100);
        builder.Property(x => x.DestinationCity).HasMaxLength(100);
        builder.Property(x => x.Details).HasMaxLength(450);

        builder.HasIndex(x => new { x.AgencyName, x.PickUpCity, x.DestinationCity ,x.PickUpDate}).IsUnique();

        builder
             .HasMany(x => x.Links)
             .WithOne(x => x.AgencyTrip)
             .HasForeignKey(x => x.AgencyId);

    }
}
