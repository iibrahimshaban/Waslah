
namespace Waslah.Presistence.EntitiesConfiguration
{
    public class StationTypeConfiguration : IEntityTypeConfiguration<StationType>
    {
        public void Configure(EntityTypeBuilder<StationType> builder)
        {
            builder.HasKey(st => st.Name);
        }
    }
}
