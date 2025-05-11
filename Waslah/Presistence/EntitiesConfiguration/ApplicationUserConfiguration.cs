using Waslah.Abstraction.Consts;

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
           .HasMany(u => u.Reports)
           .WithOne(r => r.User)
           .HasForeignKey(r => r.UserId);

            builder.HasData(new ApplicationUser
            {
                Id = DefaultUsers.UserId,
                Email = DefaultUsers.Email,
                NormalizedEmail = DefaultUsers.Email.ToUpper(),
                PasswordHash = DefaultUsers.HashedPassword,
                EmailConfirmed = true,
                LockoutEnabled = true,
                SecurityStamp = DefaultUsers.SecurityStamp,
                ConcurrencyStamp = DefaultUsers.ConcurrencyStamp,
                FirstName = DefaultUsers.FirstName,
                LastName = DefaultUsers.LastName,
                UserName = DefaultUsers.UserName,
                NormalizedUserName = DefaultUsers.UserName.ToUpper()
            });
        }
    }
}
