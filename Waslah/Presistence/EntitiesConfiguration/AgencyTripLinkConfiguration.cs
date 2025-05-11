
namespace Waslah.Presistence.EntitiesConfiguration;

public class AgencyTripLinkConfiguration : IEntityTypeConfiguration<AgencyLink>
{
    public void Configure(EntityTypeBuilder<AgencyLink> builder)
    {
        builder.ToTable("Links");
        builder
            .HasKey(x => x.Id);

        builder.HasIndex(x => new { x.Name, x.Url }).IsUnique();
    }
}
