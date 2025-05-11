
namespace Waslah.Presistence.EntitiesConfiguration;

public class ModelEncodingMapConfiguration : IEntityTypeConfiguration<ModelEncodingMap>
{
    public void Configure(EntityTypeBuilder<ModelEncodingMap> builder)
    {
        builder.HasKey(x => x.LocId);

        builder.Property(x => x.LocId)
            .ValueGeneratedNever();
            
    }
}
