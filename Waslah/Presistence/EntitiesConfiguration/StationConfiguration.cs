

namespace Waslah.Presistence.EntitiesConfiguration
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.StationType)
                .WithMany(st => st.Stations)
                .HasForeignKey(s => s.Type)
                .HasPrincipalKey(st => st.Name);

            builder.Property(s => s.Government)
                .HasMaxLength(100);
            
            builder.Property(s => s.City)
                .HasMaxLength(100);
            
            builder.Property(s => s.Government)
                .HasMaxLength(450);

            
        }
    }
}
