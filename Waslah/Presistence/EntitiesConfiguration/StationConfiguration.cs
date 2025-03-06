

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

            //builder.Property(s => s.LocationId)
            //    .HasComputedColumnSql("([GovNo]*10000+[CityNo]*100+[StationNo])",stored: true);

           // builder.Property(s => s.ModelId)
           //.HasComputedColumnSql("CONCAT([GovNo],:, [CityNo],:, [StationNo])", stored: true);

            
        }
    }
}
