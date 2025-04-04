namespace Waslah.Presistence.EntitiesConfiguration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(15);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(15);

            builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

            builder
            .HasMany(U => U.ChainedRoutes)
            .WithMany(cr => cr.Users)
            .UsingEntity<Order>(
        x => x
            .HasOne(u => u.Route)
            .WithMany(u => u.Orders)
            .HasForeignKey(u => u.RouteId),
        y => y
            .HasOne(r => r.User)
            .WithMany(r => r.Orders)
            .HasForeignKey(r => r.UserId),
        j => j
            .HasKey(k => k.Id)
            );

            builder
            .HasMany(U => U.Routes)
            .WithMany(r => r.Users)
            .UsingEntity<Report>(
        y => y
            .HasOne(r => r.Route)
            .WithMany(r => r.Reports)
            .HasForeignKey(r => r.RouteId),
        x => x
            .HasOne(u => u.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(u => u.UserId),
        j => j
            .HasKey(k => k.Id)
            );
        }
    }
}
