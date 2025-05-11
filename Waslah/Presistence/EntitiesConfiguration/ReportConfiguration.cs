
namespace Waslah.Presistence.EntitiesConfiguration
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(x => x.Sevirity).HasMaxLength(50);
            builder.Property(x => x.Type).HasMaxLength(50);
            builder.Property(x => x.UserFeedBack).HasMaxLength(1000);

            builder.HasKey(r => r.Id);
        }
    }
}
